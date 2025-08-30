using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssemblyGame
{
    public class AssemblyGameManager : MonoBehaviour
    {
        private static AssemblyGameManager instance = null;

        private int score = 0;
        private float timeRemaining = 60f;
        private int currentLevel = 1;
        private bool isLevelComplete = false;
        private int currentPartIndex = 0;

        private int[] pointsPerPartPerLevel = { 10, 15, 20 };
        private string[][] assemblyOrderPerLevel =
        {
            new string[] { "Cooler", "CPU", "RAM" },
            new string[] { "CPU", "RAM", "GraphicsCard", "Cooler" },
            new string[] { "RAM", "PowerSupply", "CPU", "GraphicsCard", "Cooler" }
        };

        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI nextPartText;
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
                    Debug.LogError("AssemblyGameManager no está instanciado. Asegúrate de tener un GameObject con este script en Level1Scene.");
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

        public void StopGame()
        {
            SetState(new PausedState());
            if (currentState is PlayingState playingState)
            {
                playingState.ResetFaultTimer();
            }
            StopAllCoroutines();
            Time.timeScale = 0f;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateReferences();
            if (scene.name == "Level1Scene" && !(currentState is PlayingState))
            {
                ResetGame();
            }
            ConfigureLevel(currentLevel);
            SetState(new PlayingState());
            Time.timeScale = 1f; // Asegura que el tiempo se reanude al cargar
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
            if (faultPrefab == null || faultParent == null)
            {
                Debug.LogWarning("faultPrefab o faultParent no están asignados. No se puede generar un fault.");
                return;
            }

            FaultCreator creator = faultCreators[Random.Range(0, faultCreators.Length)];
            IFault fault = creator.CreateFault(faultPrefab);
            if (fault != null)
            {
                (fault as MonoBehaviour).transform.SetParent(faultParent, false);
                (fault as MonoBehaviour).transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogWarning("Fallo al crear la fault.");
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
                if (partType == GetNextPart())
                {
                    currentPartIndex++;
                    UpdateNextPartText();
                }
                CheckLevelCompletion();
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
            if (director == null)
            {
                Debug.LogError("Director es null en CheckLevelCompletion. Reintentando inicialización.");
                builder = new ComputerConcreteBuilder();
                director = new AssemblyDirector(builder);
                if (director == null) return;
            }
            ComputerProduct product = director.GetResult();
            if (product == null)
            {
                Debug.LogError("Product es null en CheckLevelCompletion. Verifica la inicialización de builder.");
                return;
            }
            bool isFullyAssembled = product.IsFullyAssembled();
            Debug.Log($"¿Computadora ensamblada? {isFullyAssembled}. Estado: {product.GetPartsStatus()}");
            if (isFullyAssembled)
            {
                isLevelComplete = true;
                Debug.Log($"¡Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");
                SceneUIManager uiManager = FindObjectOfType<SceneUIManager>();
                if (uiManager != null) uiManager.ShowLevelCompletePanel(true);
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
                currentPartIndex = 0;
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
                SceneUIManager uiManager = FindObjectOfType<SceneUIManager>();
                if (uiManager != null) uiManager.ShowGameOverPanel(true);
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

        public void UpdateNextPartText()
        {
            if (nextPartText != null)
            {
                string nextPart = GetNextPart();
                string description = nextPart != null
                    ? PartDescriptionManager.Instance.GetDescription(currentLevel, nextPart)
                    : "¡Ensamblaje completo!";
                nextPartText.text = description;
            }
            else
            {
                Debug.LogWarning("nextPartText es null al intentar actualizar UI.");
            }
        }

        public string GetNextPart()
        {
            if (currentPartIndex < assemblyOrderPerLevel[currentLevel - 1].Length)
            {
                return assemblyOrderPerLevel[currentLevel - 1][currentPartIndex];
            }
            return null;
        }

        private void UpdateReferences()
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
            nextPartText = GameObject.Find("NextPartText")?.GetComponent<TextMeshProUGUI>();
            faultParent = GameObject.Find("FaultContainer")?.transform ?? GameObject.Find("Canvas")?.transform;

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
            if (nextPartText == null) Debug.LogWarning("NextPartText no encontrado en la escena actual.");
            if (faultParent == null) Debug.LogWarning("FaultContainer o Canvas no encontrado en la escena actual.");
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
            currentPartIndex = 0;
            SetTimeRemaining(60f);
            UpdateNextPartText();
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
            currentPartIndex = 0;
            timeRemaining = 60f;
            isLevelComplete = false;
            SceneManager.LoadScene("Level1Scene"); // Asegura que se cargue Level1Scene
            ConfigureLevel(currentLevel); // Configura el nivel después de cargar
        }
    }
}
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//namespace AssemblyGame
//{
//    public class AssemblyGameManager : MonoBehaviour
//    {
//        private static AssemblyGameManager instance = null;

//        private int score = 0;
//        private float timeRemaining = 60f;
//        private int currentLevel = 1;
//        private bool isLevelComplete = false;
//        private int currentPartIndex = 0;

//        private int[] pointsPerPartPerLevel = { 10, 15, 20 };
//        private string[][] assemblyOrderPerLevel =
//        {
//            new string[] { "Cooler", "CPU", "RAM" },
//            new string[] { "CPU", "RAM", "GraphicsCard", "Cooler" },
//            new string[] { "RAM", "PowerSupply", "CPU", "GraphicsCard", "Cooler" }
//        };

//        [SerializeField] private TextMeshProUGUI scoreText;
//        [SerializeField] private TextMeshProUGUI timerText;
//        [SerializeField] private TextMeshProUGUI nextPartText;
//        [SerializeField] private GameObject[] slots;
//        [SerializeField] private GameObject[] availableParts;
//        [SerializeField] private GameObject faultPrefab;
//        [SerializeField] private Transform faultParent;

//        private AssemblyDirector director;
//        private ComputerConcreteBuilder builder;
//        private IAssemblyPartsFactory partsFactory;

//        private FaultCreator[] faultCreators;

//        private IGameState currentState;

//        private AssemblyGameManager() { }

//        public static AssemblyGameManager getInstance()
//        {
//            if (instance == null)
//            {
//                instance = FindObjectOfType<AssemblyGameManager>();
//                if (instance == null)
//                {
//                    Debug.LogError("AssemblyGameManager no está instanciado. Asegúrate de tener un GameObject con este script en Level1Scene.");
//                }
//            }
//            return instance;
//        }

//        private void Awake()
//        {
//            if (instance != null && instance != this)
//            {
//                Debug.Log($"Destruyendo instancia duplicada de AssemblyGameManager en GameObject: {gameObject.name}");
//                Destroy(gameObject);
//                return;
//            }

//            instance = this;
//            DontDestroyOnLoad(gameObject);

//            builder = new ComputerConcreteBuilder();
//            director = new AssemblyDirector(builder);
//            SceneManager.sceneLoaded += OnSceneLoaded;

//            faultCreators = new FaultCreator[]
//            {
//                new PointsFaultCreator(),
//                new TimeFaultCreator()
//            };

//            ConfigureLevel(currentLevel);
//            currentState = new PlayingState();
//            currentState.OnEnter(this);
//        }

//        private void OnDestroy()
//        {
//            if (instance == this)
//            {
//                SceneManager.sceneLoaded -= OnSceneLoaded;
//                instance = null;
//            }
//        }

//        public void StopGame()
//        {
//            SetState(new PausedState());
//            if (currentState is PlayingState playingState)
//            {
//                playingState.ResetFaultTimer();
//            }
//            StopAllCoroutines();
//            Time.timeScale = 0f;
//        }

//        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//        {
//            UpdateReferences();
//            if (scene.name == "Level1Scene" && !(currentState is PlayingState))
//            {
//                ResetGame();
//            }
//            ConfigureLevel(currentLevel);
//            SetState(new PlayingState());
//            Time.timeScale = 1f;
//        }

//        private void Update()
//        {
//            currentState.Update(this);
//            currentState.HandleInput(this);
//            UpdateTimerUI();
//            UpdateScoreUI();
//            CheckTimeOut();
//        }

//        public void SetState(IGameState newState)
//        {
//            currentState.OnExit(this);
//            currentState = newState;
//            currentState.OnEnter(this);
//        }

//        public IGameState GetCurrentState()
//        {
//            return currentState;
//        }

//        public void SpawnFault()
//        {
//            if (faultPrefab == null || faultParent == null)
//            {
//                Debug.LogWarning("faultPrefab o faultParent no están asignados. No se puede generar un fault.");
//                return;
//            }

//            FaultCreator creator = faultCreators[Random.Range(0, faultCreators.Length)];
//            IFault fault = creator.CreateFault(faultPrefab);
//            if (fault != null)
//            {
//                (fault as MonoBehaviour).transform.SetParent(faultParent, false);
//                (fault as MonoBehaviour).transform.localPosition = Vector3.zero;
//            }
//            else
//            {
//                Debug.LogWarning("Fallo al crear la fault.");
//            }
//        }

//        public bool TryAddPart(string partType)
//        {
//            if (!IsPartValidForLevel(partType))
//            {
//                Debug.Log($"Error: {partType} no es una parte válida para este nivel.");
//                return false;
//            }

//            bool success = director.TryAddPart(partType);
//            if (success)
//            {
//                AddScore(pointsPerPartPerLevel[currentLevel - 1]);
//                Debug.Log($"Parte {partType} añadida correctamente. Verificando completitud del nivel...");
//                if (partType == GetNextPart())
//                {
//                    currentPartIndex++;
//                    UpdateNextPartText();
//                }
//                CheckLevelCompletion();
//            }
//            return success;
//        }

//        public void AddScore(int points)
//        {
//            score += points;
//            UpdateScoreUI();
//        }

//        public void DeductTime(float amount)
//        {
//            timeRemaining -= amount;
//            if (timeRemaining < 0) timeRemaining = 0;
//            UpdateTimerUI();
//        }

//        public void AddTime(float amount)
//        {
//            timeRemaining += amount;
//            UpdateTimerUI();
//        }

//        public void SetTimeRemaining(float time)
//        {
//            timeRemaining = time;
//            UpdateTimerUI();
//        }

//        public void CheckLevelCompletion()
//        {
//            if (director == null)
//            {
//                Debug.LogError("Director es null en CheckLevelCompletion. Reintentando inicialización.");
//                builder = new ComputerConcreteBuilder();
//                director = new AssemblyDirector(builder);
//                if (director == null) return;
//            }
//            ComputerProduct product = director.GetResult();
//            if (product == null)
//            {
//                Debug.LogError("Product es null en CheckLevelCompletion. Verifica la inicialización de builder.");
//                return;
//            }
//            bool isFullyAssembled = product.IsFullyAssembled();
//            Debug.Log($"¿Computadora ensamblada? {isFullyAssembled}. Estado: {product.GetPartsStatus()}");
//            if (isFullyAssembled)
//            {
//                isLevelComplete = true;
//                Debug.Log($"¡Nivel {currentLevel} completado! La computadora ha sido ensamblada correctamente.");
//                SceneUIManager uiManager = FindObjectOfType<SceneUIManager>();
//                if (uiManager != null) uiManager.ShowLevelCompletePanel(true);
//                SetState(new LevelCompletedState());
//            }
//        }

//        public bool IsLevelComplete()
//        {
//            return isLevelComplete;
//        }

//        public void AdvanceToNextLevel()
//        {
//            if (currentLevel < assemblyOrderPerLevel.Length)
//            {
//                currentLevel++;
//                currentPartIndex = 0;
//                SetTimeRemaining(60f);
//                SceneManager.LoadScene($"Level{currentLevel}Scene");
//                SetState(new PlayingState());
//                Time.timeScale = 1f;
//            }
//            else
//            {
//                Debug.Log("¡Has completado todos los niveles! Puntaje final: " + score);
//                ResetGame();
//            }
//        }

//        public void CheckTimeOut()
//        {
//            if (timeRemaining <= 0 && !(currentState is GameOverState))
//            {
//                SetState(new GameOverState());
//                SceneUIManager uiManager = FindObjectOfType<SceneUIManager>();
//                if (uiManager != null) uiManager.ShowGameOverPanel(true);
//            }
//        }

//        public float GetTimeRemaining()
//        {
//            return timeRemaining;
//        }

//        public int GetCurrentLevel()
//        {
//            return currentLevel;
//        }

//        public GameObject[] GetSlots()
//        {
//            return slots;
//        }

//        public void UpdateScoreUI()
//        {
//            if (scoreText != null)
//            {
//                scoreText.text = "Puntaje: " + score.ToString();
//            }
//            else
//            {
//                Debug.LogWarning("ScoreText es null al intentar actualizar UI.");
//            }
//        }

//        public void UpdateTimerUI()
//        {
//            if (timerText != null)
//            {
//                timerText.text = Mathf.Ceil(timeRemaining).ToString();
//            }
//            else
//            {
//                Debug.LogWarning("TimerText es null al intentar actualizar UI.");
//            }
//        }

//        public void UpdateNextPartText()
//        {
//            if (nextPartText != null)
//            {
//                string nextPart = GetNextPart();
//                string description = nextPart != null
//                    ? PartDescriptionManager.Instance.GetDescription(currentLevel, nextPart)
//                    : "¡Ensamblaje completo!";
//                nextPartText.text = description;
//            }
//            else
//            {
//                Debug.LogWarning("nextPartText es null al intentar actualizar UI.");
//            }
//        }

//        public string GetNextPart()
//        {
//            if (currentPartIndex < assemblyOrderPerLevel[currentLevel - 1].Length)
//            {
//                return assemblyOrderPerLevel[currentLevel - 1][currentPartIndex];
//            }
//            return null;
//        }

//        private void UpdateReferences()
//        {
//            scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
//            timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
//            nextPartText = GameObject.Find("NextPartText")?.GetComponent<TextMeshProUGUI>();
//            faultParent = GameObject.Find("FaultContainer")?.transform ?? GameObject.Find("Canvas")?.transform;

//            slots = new GameObject[]
//            {
//                GameObject.Find("Slot_Cooler"),
//                GameObject.Find("Slot_CPU"),
//                GameObject.Find("Slot_RAM"),
//                GameObject.Find("Slot_GraphicsCard"),
//                GameObject.Find("Slot_PowerSupply")
//            }.Where(slot => slot != null).ToArray();

//            availableParts = GameObject.FindGameObjectsWithTag("AssemblyPart");

//            if (scoreText == null) Debug.LogWarning("ScoreText no encontrado en la escena actual.");
//            if (timerText == null) Debug.LogWarning("TimerText no encontrado en la escena actual.");
//            if (nextPartText == null) Debug.LogWarning("NextPartText no encontrado en la escena actual.");
//            if (faultParent == null) Debug.LogWarning("FaultContainer o Canvas no encontrado en la escena actual.");
//        }

//        private void ConfigureLevel(int level)
//        {
//            isLevelComplete = false;
//            builder.Reset();
//            director.Reset();
//            builder.SetAssemblyOrder(assemblyOrderPerLevel[level - 1]);
//            partsFactory = level switch
//            {
//                1 => new Level1PartsFactory(),
//                2 => new Level2PartsFactory(),
//                3 => new Level3PartsFactory(),
//                _ => new Level1PartsFactory()
//            };
//            currentPartIndex = 0;
//            SetTimeRemaining(60f);
//            UpdateNextPartText();
//            Debug.Log($"Iniciando nivel {level}. Orden de ensamblaje: {string.Join(" -> ", assemblyOrderPerLevel[level - 1])}");
//            ValidatePartsForLevel();
//        }

//        private void ValidatePartsForLevel()
//        {
//            if (availableParts == null) return;
//            foreach (var part in availableParts)
//            {
//                if (part == null) continue;
//                AssemblyPart assemblyPart = part.GetComponent<AssemblyPart>();
//                if (assemblyPart == null) continue;

//                bool isValid = IsPartValidForLevel(assemblyPart.PartType);
//                part.SetActive(isValid);
//            }
//        }

//        private bool IsPartValidForLevel(string partType)
//        {
//            bool isValid = partType switch
//            {
//                "Cooler" => partsFactory.CreateCooler(),
//                "CPU" => partsFactory.CreateCPU(),
//                "RAM" => partsFactory.CreateRAM(),
//                "RAM_Advanced" => partsFactory.CreateRAM_Advanced(),
//                "GraphicsCard" => partsFactory.CreateGraphicsCard(),
//                "PowerSupply" => partsFactory.CreatePowerSupply(),
//                _ => false
//            };
//            return isValid;
//        }

//        public void ResetGame()
//        {
//            score = 0;
//            currentLevel = 1;
//            currentPartIndex = 0;
//            timeRemaining = 60f;
//            isLevelComplete = false;
//            ConfigureLevel(currentLevel);
//            // Nota: La escena será sobrescrita por Menu_Ensamblaje desde OnExit
//        }
//    }
//}
