using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

namespace AssemblyGame
{
    public class AssemblyGameManager : MonoBehaviour
    {
        // Singleton: Variable est�tica privada que almacena la �nica instancia de AssemblyGameManager.
        // Garantiza que solo haya un administrador del juego en todo momento.
        private static AssemblyGameManager instance = null;

        // Variables de estado del juego
        private int score = 0; // Almacena el puntaje del jugador.
        private float timeRemaining = 60f; // Tiempo restante en segundos para completar el nivel.
        private int currentLevel = 1; // Nivel actual del juego.
        private bool isLevelComplete = false; // Indica si el nivel actual est� completado.
        private bool isGameOver = false; // Indica si el juego ha terminado.

        // Configuraci�n de niveles
     
        private int[] pointsPerPartPerLevel = { 10, 15, 20 };
        // Orden de ensamblaje de las partes para cada nivel.
        private string[][] assemblyOrderPerLevel =
        {
            new string[] { "Cooler", "CPU", "RAM" },
            new string[] { "Cooler", "CPU", "GraphicsCard", "RAM" },
            new string[] { "Cooler", "GraphicsCard", "CPU", "RAM", "PowerSupply" }
        };

       
        [SerializeField] private TextMeshProUGUI scoreText; 
        [SerializeField] private TextMeshProUGUI timerText; 
        [SerializeField] private TextMeshProUGUI gameOverText; 
        [SerializeField] private GameObject[] slots; 
        [SerializeField] private GameObject[] availableParts; 
        [SerializeField] private GameObject faultPrefab; 
        [SerializeField] private Transform faultParent; 

        // Builder
        private AssemblyDirector director; // Director del patr�n Builder, 
        private ComputerConcreteBuilder builder; // Builder concreto para construir la computadora.
        private IAssemblyPartsFactory partsFactory; // F�brica abstracta para determinar qu� partes est�n disponibles por nivel.

        // Gesti�n de fallos (usando Factory Method)
        private float faultSpawnTimer = 0f; // Temporizador para controlar la generaci�n de fallos.
        private float faultSpawnInterval = 10f; // Intervalo de tiempo  entre la generaci�n de fallos.
        private FaultCreator[] faultCreators; // Arreglo de creadores de fallos (PointsFaultCreator, TimeFaultCreator).

        // Constructor privado (parte del patr�n Singleton).
        // Evita que se creen instancias de AssemblyGameManager desde fuera de la clase.
        private AssemblyGameManager() { }

        // M�todo est�tico p�blico para acceder a la �nica instancia (patr�n Singleton).
        // asegura que persista entre escenas.
        public static AssemblyGameManager getInstance()
        {
            if (instance == null)
            {
                GameObject gameManagerObject = new GameObject("AssemblyGameManager");
                instance = gameManagerObject.AddComponent<AssemblyGameManager>();
                DontDestroyOnLoad(gameManagerObject); // Persiste el objeto entre escenas.
            }
            return instance;
        }

        
        // Implementa la l�gica del Singleton 
        private void Awake()
        {
            // Singleton: Verifica si ya existe una instancia.
            // Si existe otra, destruye este objeto para garantizar una sola instancia.
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject); 

            // Inicializa el Builder y el Director para el ensamblaje de la computadora.
            builder = new ComputerConcreteBuilder();
            director = new AssemblyDirector(builder);

            // Suscribe el m�todo OnSceneLoaded al evento de carga de escenas.
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Inicializa los creadores de fallos (Factory Method).
            faultCreators = new FaultCreator[]
            {
                new PointsFaultCreator(),
                new TimeFaultCreator()
            };
        }

        // M�todo se ejecuta al destruir el objeto.
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // M�todo que se ejecuta cada vez que se carga una nueva escena.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateReferences(); // Actualiza las referencias a los objetos de la escena.
            ConfigureLevel(currentLevel); // Configura el nivel actual.
            UpdateScoreUI(); // Actualiza el texto del puntaje en la UI.
            UpdateTimerUI(); // Actualiza el texto del temporizador en la UI.
        }

        
        // Gestiona la l�gica principal del juego: temporizador, finalizaci�n de nivel y generaci�n de fallos.
        private void Update()
        {
            // Si el nivel est� completado, el juego ha terminado o se acab� el tiempo, no hace nada.
            if (isLevelComplete || isGameOver || timeRemaining <= 0) return;

            // Reduce el tiempo restante y actualiza la UI.
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            // Si se acaba el tiempo, termina el juego.
            if (timeRemaining <= 0)
            {
                isGameOver = true;
                Debug.Log("�Tiempo agotado! Fin del juego.");
                ShowGameOver();
            }

            // Verifica si el nivel est� completado.
            CheckLevelCompletion();
            // Genera fallos (si corresponde).
            SpawnFaults();
        }

        
        // Usa el patr�n Factory Method para crear los fallos.
        private void SpawnFaults()
        {
            if (currentLevel < 2) return; // Los fallos solo aparecen a partir del Nivel 2.

            faultSpawnTimer += Time.deltaTime;
            if (faultSpawnTimer >= faultSpawnInterval)
            {
                // Selecciona un creador aleatorio y crea un fallo.
                FaultCreator creator = faultCreators[Random.Range(0, faultCreators.Length)];
                IFault fault = creator.CreateFault(faultPrefab);
                faultParent = faultParent ?? GameObject.Find("Canvas").transform;
                (fault as MonoBehaviour).transform.SetParent(faultParent, false);

                faultSpawnTimer = 0f; // Reinicia el temporizador.
            }
        }

        
        // Usa el patr�n Builder para gestionar el ensamblaje.
        public bool TryAddPart(string partType)
        {
            // Verifica si la parte es v�lida para el nivel actual (usando Abstract Factory).
            if (!IsPartValidForLevel(partType))
            {
                Debug.Log($"Error: {partType} no es una parte v�lida para este nivel.");
                return false;
            }

            // Intenta a�adir la parte usando el Director.
            bool success = director.TryAddPart(partType);
            if (success)
            {
                // Si se a�adi� correctamente, suma puntos seg�n el nivel.
                AddScore(pointsPerPartPerLevel[currentLevel - 1]);
            }
            return success;
        }

        // A�ade o resta puntos al puntaje del jugador y actualiza la UI.
        public void AddScore(int points)
        {
            score += points;
            UpdateScoreUI();
        }

        // Resta tiempo al temporizador (usado por TimeFault) y actualiza la UI.
        public void DeductTime(float amount)
        {
            timeRemaining -= amount;
            if (timeRemaining < 0) timeRemaining = 0;
            UpdateTimerUI();
        }

        // Devuelve el arreglo de slots (usado por los creadores de fallos).
        public GameObject[] GetSlots()
        {
            return slots;
        }

        // Verifica si si la computadora est� completamente ensamblada.
        // Si se completa, avanza al siguiente nivel o termina el juego.
        private void CheckLevelCompletion()
        {
            ComputerProduct product = director.GetResult();
            if (product.IsFullyAssembled())
            {
                isLevelComplete = true;
                Debug.Log($"�Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");

                timeRemaining += 10f; // A�ade tiempo extra al completar un nivel.

                // Si hay m�s niveles, avanza al siguiente.
                if (currentLevel < assemblyOrderPerLevel.Length)
                {
                    currentLevel++;
                    SceneManager.LoadScene($"Level{currentLevel}Scene");
                }
                else
                {
                    // Si no hay m�s niveles, termina el juego.
                    Debug.Log("�Has completado todos los niveles! Puntaje final: " + score);
                    ResetGame();
                }
            }
        }

        // Muestra el texto de "Game Over" y reinicia el juego despu�s de 3 segundos.
        private void ShowGameOver()
        {
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
            }

            Invoke(nameof(ResetGame), 3f);
        }

        // Actualiza las referencias a los objetos de la escena (UI, slots, partes, etc.).
        private void UpdateReferences()
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
            gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();
            faultParent = GameObject.Find("Canvas")?.transform;

            slots = new GameObject[]
            {
                GameObject.Find("Slot_Cooler"),
                GameObject.Find("Slot_CPU"),
                GameObject.Find("Slot_RAM"),
                GameObject.Find("Slot_GraphicsCard"),
                GameObject.Find("Slot_PowerSupply")
            }.Where(slot => slot != null).ToArray();

            availableParts = GameObject.FindGameObjectsWithTag("AssemblyPart");

            if (scoreText == null || timerText == null || gameOverText == null || faultParent == null)
            {
                Debug.LogWarning("No se encontraron ScoreText, TimerText, GameOverText o Canvas en la escena. Aseg�rate de que existan y est�n nombrados correctamente.");
            }
            else
            {
                gameOverText.gameObject.SetActive(false);
            }
        }

        // Actualiza el texto del puntaje en la UI.
        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Puntaje: " + score.ToString();
            }
            else
            {
                Debug.LogWarning("ScoreText no est� asignado. No se puede actualizar el puntaje en la UI.");
            }
        }

        // Actualiza el texto del temporizador en la UI.
        private void UpdateTimerUI()
        {
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
            }
            else
            {
                Debug.LogWarning("TimerText no est� asignado. No se puede actualizar el temporizador en la UI.");
            }
        }

        // Configura el nivel actual: reinicia estados, configura el orden de ensamblaje y las partes disponibles.
        private void ConfigureLevel(int level)
        {
            isLevelComplete = false;
            isGameOver = false;
            faultSpawnTimer = 0f;
            director.Reset();
            builder.SetAssemblyOrder(assemblyOrderPerLevel[level - 1]);
            // Usa el patr�n Abstract Factory para determinar las partes disponibles.
            partsFactory = level switch
            {
                1 => new Level1PartsFactory(),
                2 => new Level2PartsFactory(),
                3 => new Level3PartsFactory(),
                _ => new Level1PartsFactory()
            };
            Debug.Log($"Iniciando nivel {level}. Orden de ensamblaje: {string.Join(" -> ", assemblyOrderPerLevel[level - 1])}");
            ValidatePartsForLevel();
        }

        // Activa o desactiva las partes disponibles seg�n el nivel (usando Abstract Factory).
        private void ValidatePartsForLevel()
        {
            if (availableParts == null) return;
            foreach (var part in availableParts)
            {
                if (part == null) continue;
                AssemblyPart assemblyPart = part.GetComponent<AssemblyPart>();
                if (assemblyPart == null) continue;

                bool isValid = IsPartValidForLevel(assemblyPart.PartType);
                part.SetActive(isValid);
                if (!isValid)
                {
                    Debug.Log($"Parte {assemblyPart.PartType} desactivada porque no es v�lida para el nivel {currentLevel}.");
                }
            }
        }

        // Verifica si una parte es v�lida para el nivel actual (usando Abstract Factory).
        private bool IsPartValidForLevel(string partType)
        {
            bool isValid = partType switch
            {
                "Cooler" => partsFactory.CreateCooler(),
                "CPU" => partsFactory.CreateCPU(),
                "RAM" => partsFactory.CreateRAM(),
                "RAM_Advanced" => partsFactory.CreateRAM_Advanced(),
                "GraphicsCard" => partsFactory.CreateGraphicsCard(),
                "PowerSupply" => partsFactory.CreatePowerSupply(),
                _ => false
            };
            return isValid;
        }

        // Reinicia el juego al estado inicial (puntaje, nivel, tiempo, etc.).
        private void ResetGame()
        {
            score = 0;
            currentLevel = 1;
            timeRemaining = 60f;
            isGameOver = false;
            SceneManager.LoadScene("Level1Scene");
        }
    }
}