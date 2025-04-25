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
        private bool isGameOver = false;

        private int[] pointsPerPartPerLevel = { 10, 15, 20 };
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

        private AssemblyDirector director;
        private ComputerConcreteBuilder builder;
        private IAssemblyPartsFactory partsFactory;

        private float faultSpawnTimer = 0f;
        private float faultSpawnInterval = 10f;
        private FaultCreator[] faultCreators;

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

            faultCreators = new FaultCreator[]
            {
                new PointsFaultCreator(),
                new TimeFaultCreator()
            };
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateReferences();
            ConfigureLevel(currentLevel);
            UpdateScoreUI();
            UpdateTimerUI();
        }

        private void Update()
        {
            if (isLevelComplete || isGameOver || timeRemaining <= 0) return;

            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                isGameOver = true;
                Debug.Log("¡Tiempo agotado! Fin del juego.");
                ShowGameOver();
            }

            CheckLevelCompletion();
            SpawnFaults();
        }

        private void SpawnFaults()
        {
            if (currentLevel < 2) return;

            faultSpawnTimer += Time.deltaTime;
            if (faultSpawnTimer >= faultSpawnInterval)
            {
                FaultCreator creator = faultCreators[Random.Range(0, faultCreators.Length)];
                IFault fault = creator.CreateFault(faultPrefab);
                faultParent = faultParent ?? GameObject.Find("Canvas").transform;
                (fault as MonoBehaviour).transform.SetParent(faultParent, false);

                faultSpawnTimer = 0f;
            }
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

        public void DeductTime(float amount)
        {
            timeRemaining -= amount;
            if (timeRemaining < 0) timeRemaining = 0;
            UpdateTimerUI();
        }

        public GameObject[] GetSlots() // Nuevo método público
        {
            return slots;
        }

        private void CheckLevelCompletion()
        {
            ComputerProduct product = director.GetResult();
            if (product.IsFullyAssembled())
            {
                isLevelComplete = true;
                Debug.Log($"¡Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");

                timeRemaining += 10f;

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

        private void ShowGameOver()
        {
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
            }

            Invoke(nameof(ResetGame), 3f);
        }

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
                Debug.LogWarning("No se encontraron ScoreText, TimerText, GameOverText o Canvas en la escena. Asegúrate de que existan y estén nombrados correctamente.");
            }
            else
            {
                gameOverText.gameObject.SetActive(false);
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
            isLevelComplete = false;
            isGameOver = false;
            faultSpawnTimer = 0f;
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