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
        private float timeRemaining = 5f;
        private int currentLevel = 1;
        private bool isLevelComplete = false;

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
        [SerializeField] private GameObject faultPrefab;
        [SerializeField] private Transform faultParent;

        private AssemblyDirector director;
        private ComputerConcreteBuilder builder;
        private IAssemblyPartsFactory partsFactory;

        private FaultCreator[] faultCreators;

        private IGameState currentState;

        private AssemblyGameManager() { }

        public static AssemblyGameManager getInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AssemblyGameManager>();
                if (instance == null)
                {
                    Debug.LogError("AssemblyGameManager no está instanciado. Asegúrate de tener un GameObject con este script en la escena inicial (Level1Scene).");
                }
            }
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.Log($"Destruyendo instancia duplicada de AssemblyGameManager en GameObject: {gameObject.name}");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            if (FindObjectOfType<UIManagerState>() == null)
            {
                GameObject uiManagerObject = new GameObject("UIManager");
                uiManagerObject.AddComponent<UIManagerState>();
                DontDestroyOnLoad(uiManagerObject);
                Debug.Log("UIManagerState creado dinámicamente.");
            }

            if (faultPrefab == null)
            {
                Debug.LogWarning("FaultPrefab no está asignado en AssemblyGameManager al iniciar. Asegúrate de asignarlo en el Inspector en Level1Scene.");
            }

            builder = new ComputerConcreteBuilder();
            director = new AssemblyDirector(builder);
            SceneManager.sceneLoaded += OnSceneLoaded;

            faultCreators = new FaultCreator[]
            {
                new PointsFaultCreator(),
                new TimeFaultCreator()
            };

            ConfigureLevel(currentLevel);
            currentState = new PlayingState();
            currentState.OnEnter(this);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateReferences();
            ConfigureLevel(currentLevel);
            SetState(new PlayingState());
            Time.timeScale = 1f;
        }

        private void Update()
        {
            currentState.Update(this);
            currentState.HandleInput(this);
            UpdateTimerUI();
            UpdateScoreUI();
            CheckTimeOut();
        }

        public void SetState(IGameState newState)
        {
            currentState.OnExit(this);
            currentState = newState;
            currentState.OnEnter(this);
        }

        public IGameState GetCurrentState()
        {
            return currentState;
        }

        public void SpawnFault()
        {
            if (faultPrefab == null)
            {
                Debug.LogWarning("FaultPrefab no está asignado en AssemblyGameManager. No se puede generar un fault.");
                return;
            }

            FaultCreator creator = faultCreators[Random.Range(0, faultCreators.Length)];
            IFault fault = creator.CreateFault(faultPrefab);
            faultParent = faultParent ?? GameObject.Find("Canvas").transform;
            if (faultParent != null)
            {
                (fault as MonoBehaviour).transform.SetParent(faultParent, false);
            }
            else
            {
                Debug.LogWarning("No se encontró el Canvas para asignar como padre del fault.");
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
                Debug.Log($"Parte {partType} añadida correctamente. Verificando completitud del nivel...");
                CheckLevelCompletion();
                if (IsLevelComplete())
                {
                    SetState(new LevelCompletedState());
                }
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

        public void AddTime(float amount)
        {
            timeRemaining += amount;
            UpdateTimerUI();
        }

        public void SetTimeRemaining(float time)
        {
            timeRemaining = time;
            UpdateTimerUI();
        }

        public void CheckLevelCompletion()
        {
            ComputerProduct product = director.GetResult();
            bool isFullyAssembled = product.IsFullyAssembled();
            Debug.Log($"¿Computadora ensamblada? {isFullyAssembled}. Estado: {product.GetPartsStatus()}");
            if (isFullyAssembled)
            {
                isLevelComplete = true;
                Debug.Log($"¡Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");
                UIManagerState.Instance.ShowLevelComplete();
                SetState(new LevelCompletedState());
            }
        }

        public bool IsLevelComplete()
        {
            return isLevelComplete;
        }

        public void AdvanceToNextLevel()
        {
            if (currentLevel < assemblyOrderPerLevel.Length)
            {
                currentLevel++;
                SetTimeRemaining(60f);
                SceneManager.LoadScene($"Level{currentLevel}Scene");
                SetState(new PlayingState());
                Time.timeScale = 1f;
            }
            else
            {
                Debug.Log("¡Has completado todos los niveles! Puntaje final: " + score);
                ResetGame();
            }
        }

        public void CheckTimeOut()
        {
            if (timeRemaining <= 0 && !(currentState is GameOverState))
            {
                SetState(new GameOverState());
                UIManagerState.Instance.ShowGameOver();
            }
        }

        public float GetTimeRemaining()
        {
            return timeRemaining;
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public GameObject[] GetSlots()
        {
            return slots;
        }

        public void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = "Puntaje: " + score.ToString();
            }
            else
            {
                Debug.LogWarning("ScoreText es null al intentar actualizar UI.");
            }
        }

        public void UpdateTimerUI()
        {
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
            }
            else
            {
                Debug.LogWarning("TimerText es null al intentar actualizar UI.");
            }
        }

        private void UpdateReferences()
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
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

            if (scoreText == null) Debug.LogWarning("ScoreText no encontrado en la escena actual.");
            if (timerText == null) Debug.LogWarning("TimerText no encontrado en la escena actual.");
            if (faultParent == null) Debug.LogWarning("Canvas no encontrado en la escena actual.");
        }

        private void ConfigureLevel(int level)
        {
            isLevelComplete = false;
            builder.Reset();
            director.Reset();
            builder.SetAssemblyOrder(assemblyOrderPerLevel[level - 1]);
            partsFactory = level switch
            {
                1 => new Level1PartsFactory(),
                2 => new Level2PartsFactory(),
                3 => new Level3PartsFactory(),
                _ => new Level1PartsFactory()
            };
            SetTimeRemaining(60f);
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

        public void ResetGame()
        {
            score = 0;
            currentLevel = 1;
            timeRemaining = 60f;
            SceneManager.LoadScene("Level1Scene");
        }
    }
}