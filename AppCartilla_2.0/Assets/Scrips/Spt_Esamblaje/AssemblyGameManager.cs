using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

namespace AssemblyGame
{
    public class AssemblyGameManager : MonoBehaviour
    {
        private static AssemblyGameManager instance = null;

        private int score = 0;
        private float timeRemaining = 60f;
        private int currentLevel = 1;
        private bool isLevelComplete = false;

        private float[] timePerLevel = { 60f, 50f, 40f };
        private int[] pointsPerPartPerLevel = { 10, 15, 20 };
        private string[][] assemblyOrderPerLevel =
        {
            new string[] { "Cooler", "CPU", "RAM" },
            new string[] { "Cooler", "CPU", "GraphicsCard", "RAM" },
            new string[] { "Cooler", "GraphicsCard", "CPU", "RAM", "PowerSupply" }
        };

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject[] slots;
        [SerializeField] private GameObject[] availableParts;

        private AssemblyDirector director;
        private ComputerConcreteBuilder builder;
        private IAssemblyPartsFactory partsFactory;

        private AssemblyGameManager() { }

        public static AssemblyGameManager getInstance()
        {
            if (instance == null)
            {
                GameObject gameManagerObject = new GameObject("AssemblyGameManager");
                instance = gameManagerObject.AddComponent<AssemblyGameManager>();
                DontDestroyOnLoad(gameManagerObject);
            }
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            builder = new ComputerConcreteBuilder();
            director = new AssemblyDirector(builder);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateReferences();
            ConfigureLevel(currentLevel); // Llamar ConfigureLevel después de actualizar referencias
            UpdateScoreUI();
            UpdateTimerUI();
        }

        private void Update()
        {
            if (isLevelComplete || timeRemaining <= 0) return;
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
            if (timeRemaining <= 0)
            {
                Debug.Log("¡Tiempo agotado! Fin del juego.");
                ResetLevel();
            }
            CheckLevelCompletion();
        }

        public bool TryAddPart(string partType)
        {
            if (!IsPartValidForLevel(partType))
            {
                Debug.Log($"Error: {partType} no es una parte válida para este nivel.");
                return false;
            }

            bool success = director.TryAddPart(partType);
            if (success)
            {
                AddScore(pointsPerPartPerLevel[currentLevel - 1]);
            }
            return success;
        }

        public void AddScore(int points)
        {
            score += points;
            UpdateScoreUI();
        }

        private void CheckLevelCompletion()
        {
            ComputerProduct product = director.GetResult();
            if (product.IsFullyAssembled())
            {
                isLevelComplete = true;
                Debug.Log($"¡Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");
                if (currentLevel < assemblyOrderPerLevel.Length)
                {
                    currentLevel++;
                    SceneManager.LoadScene($"Level{currentLevel}Scene");
                }
                else
                {
                    Debug.Log("¡Has completado todos los niveles! Puntaje final: " + score);
                    ResetGame();
                }
            }
        }

        private void UpdateReferences()
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();

            slots = new GameObject[]
            {
                GameObject.Find("Slot_Cooler"),
                GameObject.Find("Slot_CPU"),
                GameObject.Find("Slot_RAM"),
                GameObject.Find("Slot_GraphicsCard"),
                GameObject.Find("Slot_PowerSupply")
            }.Where(slot => slot != null).ToArray();

            availableParts = GameObject.FindGameObjectsWithTag("AssemblyPart");

            if (scoreText == null || timerText == null)
            {
                Debug.LogWarning("No se encontraron ScoreText o TimerText en la escena. Asegúrate de que existan y estén nombrados correctamente.");
            }
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Puntaje: " + score.ToString();
            }
            else
            {
                Debug.LogWarning("ScoreText no está asignado. No se puede actualizar el puntaje en la UI.");
            }
        }

        private void UpdateTimerUI()
        {
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
            }
            else
            {
                Debug.LogWarning("TimerText no está asignado. No se puede actualizar el temporizador en la UI.");
            }
        }

        private void ConfigureLevel(int level)
        {
            isLevelComplete = false; // Asegurarse de que isLevelComplete se reinicie
            timeRemaining = timePerLevel[level - 1];
            director.Reset();
            builder.SetAssemblyOrder(assemblyOrderPerLevel[level - 1]);
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
                    Debug.Log($"Parte {assemblyPart.PartType} desactivada porque no es válida para el nivel {currentLevel}.");
                }
            }
        }

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

        private void ResetLevel()
        {
            SceneManager.LoadScene($"Level{currentLevel}Scene");
        }

        private void ResetGame()
        {
            score = 0;
            currentLevel = 1;
            SceneManager.LoadScene("Level1Scene");
        }
    }
}