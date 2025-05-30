using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssemblyGame
{
    public class UIManagerState : MonoBehaviour
    {
        public static UIManagerState Instance { get; private set; }

        private AssemblyGameManager gameManager;
        private SceneUIManager currentSceneUIManager;
        private bool isPaused = false;

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
            currentSceneUIManager = FindObjectOfType<SceneUIManager>();
            if (currentSceneUIManager == null)
            {
                Debug.LogError("SceneUIManager no encontrado en la escena actual: " + scene.name + ". Asegúrate de asignarlo.");
            }
        }

        public void OnPause()
        {
            if (isPaused) return;
            isPaused = true;

            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowPauseCanvas(true);
                gameManager.SetState(new PausedState());
                Time.timeScale = 0f;
            }
            else
            {
                Debug.LogWarning("currentSceneUIManager es null en OnPause. No se puede mostrar el PauseCanvas.");
            }
        }

        public void OnContinue()
        {
            if (!isPaused) return;
            isPaused = false;

            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowPauseCanvas(false);
                Time.timeScale = 1f;
                gameManager.SetState(new PlayingState());
            }
            else
            {
                Debug.LogWarning("currentSceneUIManager es null en OnContinue. No se puede ocultar el PauseCanvas.");
            }
        }

        public void OnNextLevel()
        {
            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowLevelCompleteCanvas(false);
                Time.timeScale = 1f;
                gameManager.AdvanceToNextLevel();
            }
        }

        public void OnRetry()
        {
            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowLevelCompleteCanvas(false);
                Time.timeScale = 1f;
                gameManager.ResetGame();
            }
        }

        public void OnExit()
        {
            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowLevelCompleteCanvas(false);
                Time.timeScale = 1f;
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        public void ShowLevelComplete()
        {
            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowLevelCompleteCanvas(true);
                Time.timeScale = 0f;
            }
            else
            {
                Debug.LogWarning("currentSceneUIManager es null en ShowLevelComplete. No se puede mostrar el LevelCompleteCanvas.");
            }
        }

        public void ShowGameOver()
        {
            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowGameOverCanvas(true);
                Time.timeScale = 0f;
            }
            else
            {
                Debug.LogWarning("currentSceneUIManager es null en ShowGameOver. No se puede mostrar el GameOverCanvas.");
            }
        }
    }
}