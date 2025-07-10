using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssemblyGame
{
    public class UIManagerState : MonoBehaviour
    {
        public static UIManagerState Instance { get; private set; }

        private AssemblyGameManager gameManager;
        private bool isPaused = false;

        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private GameObject levelCompleteCanvas;
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private TextMeshProUGUI nextPartText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            gameManager = AssemblyGameManager.getInstance();
            if (gameManager == null)
            {
                Debug.LogError("AssemblyGameManager no encontrado.");
            }
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Inicializa los canvases como inactivos
            InitializeCanvases();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Revalida las referencias y actualiza el texto
            ValidateReferences();
            UpdateNextPartText();
        }

        private void InitializeCanvases()
        {
            if (pauseCanvas) pauseCanvas.SetActive(false);
            else Debug.LogWarning("PauseCanvas no asignado en UIManagerState.");
            if (levelCompleteCanvas) levelCompleteCanvas.SetActive(false);
            else Debug.LogWarning("LevelCompleteCanvas no asignado en UIManagerState.");
            if (gameOverCanvas) gameOverCanvas.SetActive(false);
            else Debug.LogWarning("GameOverCanvas no asignado en UIManagerState.");
            if (nextPartText) nextPartText.gameObject.SetActive(true);
            else Debug.LogWarning("NextPartText no asignado en UIManagerState.");
        }

        private void ValidateReferences()
        {
            // Reasigna referencias si se pierden (por ejemplo, al recargar escena)
            if (pauseCanvas == null) pauseCanvas = transform.Find("PauseCanvas")?.gameObject;
            if (levelCompleteCanvas == null) levelCompleteCanvas = transform.Find("LevelCompleteCanvas")?.gameObject;
            if (gameOverCanvas == null) gameOverCanvas = transform.Find("GameOverCanvas")?.gameObject;
            if (nextPartText == null) nextPartText = transform.Find("NextPartText")?.GetComponent<TextMeshProUGUI>();
        }

        public void OnPause()
        {
            if (isPaused) return;
            isPaused = true;

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(true);
                gameManager.SetState(new PausedState());
                Time.timeScale = 0f;
                Debug.Log("PauseCanvas activado en escena: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogError("PauseCanvas no está asignado o encontrado en UIManagerState.");
            }
        }

        public void OnContinue()
        {
            if (!isPaused) return;
            isPaused = false;

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(false);
                Time.timeScale = 1f;
                gameManager.SetState(new PlayingState());
                Debug.Log("PauseCanvas desactivado en escena: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogError("PauseCanvas no está asignado o encontrado en UIManagerState.");
            }
        }

        public void OnNextLevel()
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(false);
                Time.timeScale = 1f;
                gameManager.AdvanceToNextLevel();
                Debug.Log("NextLevel activado en escena: " + SceneManager.GetActiveScene().name);
            }
        }

        public void OnRetry()
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(false);
                Time.timeScale = 1f;
                gameManager.ResetGame();
                Debug.Log("Retry activado en escena: " + SceneManager.GetActiveScene().name);
            }
        }

        public void OnExit()
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(false);
                Time.timeScale = 1f;
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Debug.Log("Exit activado en escena: " + SceneManager.GetActiveScene().name);
            }
        }

        public void OnExitToMenu()
        {
            if (pauseCanvas != null) pauseCanvas.SetActive(false);
            if (levelCompleteCanvas != null) levelCompleteCanvas.SetActive(false);

            if (gameManager != null)
            {
                gameManager.StopGame();
                gameManager.ResetGame();
            }

            SceneManager.LoadScene("MenuScene"); // Cambia "MenuScene" por el nombre real
            Debug.Log("ExitToMenu activado, cargando MenuScene");
        }

        public void ShowLevelComplete()
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("ShowLevelComplete activado en escena: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogError("LevelCompleteCanvas no está asignado o encontrado en UIManagerState.");
            }
        }

        public void ShowGameOver()
        {
            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("ShowGameOver activado en escena: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogError("GameOverCanvas no está asignado o encontrado en UIManagerState.");
            }
        }

        private void UpdateNextPartText()
        {
            if (nextPartText != null && gameManager != null)
            {
                string nextPart = gameManager.GetNextPart();
                string description = nextPart != null
                    ? "Siguiente parte: " + nextPart
                    : "¡Ensamblaje completo!";
                nextPartText.text = description;
                Debug.Log("NextPartText actualizado a: " + description + " en escena: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogWarning("NextPartText o gameManager no asignado en UpdateNextPartText.");
            }
        }
    }
}