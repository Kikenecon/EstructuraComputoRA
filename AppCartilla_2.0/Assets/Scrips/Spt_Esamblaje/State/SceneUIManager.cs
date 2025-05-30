using UnityEngine;
using UnityEngine.UI;

namespace AssemblyGame
{
    public class SceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private GameObject levelCompleteCanvas;
        [SerializeField] private GameObject gameOverCanvas;
        private UIManagerState uiManager;

        private void Awake()
        {
            uiManager = UIManagerState.Instance;
            if (uiManager == null)
            {
                Debug.LogError("UIManagerState no encontrado. Asegúrate de que exista en la escena inicial.");
                return;
            }

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(false);
            }
            else
            {
                Debug.LogWarning("PauseCanvas no asignado en SceneUIManager.");
            }

            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(false);
            }
            else
            {
                Debug.LogWarning("LevelCompleteCanvas no asignado en SceneUIManager.");
            }

            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(false);
            }
            else
            {
                Debug.LogWarning("GameOverCanvas no asignado en SceneUIManager.");
            }

            SetupButtonEvents();
        }

        private void SetupButtonEvents()
        {
            if (levelCompleteCanvas != null)
            {
                Button nextLevelButton = levelCompleteCanvas.transform.Find("NextLevelButton")?.GetComponent<Button>();
                if (nextLevelButton != null)
                {
                    nextLevelButton.onClick.RemoveAllListeners();
                    nextLevelButton.onClick.AddListener(() => uiManager.OnNextLevel());
                }
                else
                {
                    Debug.LogWarning("NextLevelButton no encontrado en LevelCompleteCanvas.");
                }

                Button retryButton = levelCompleteCanvas.transform.Find("RetryButton")?.GetComponent<Button>();
                if (retryButton != null)
                {
                    retryButton.onClick.RemoveAllListeners();
                    retryButton.onClick.AddListener(() => uiManager.OnRetry());
                }
                else
                {
                    Debug.LogWarning("RetryButton no encontrado en LevelCompleteCanvas.");
                }

                Button exitButton = levelCompleteCanvas.transform.Find("ExitButton")?.GetComponent<Button>();
                if (exitButton != null)
                {
                    exitButton.onClick.RemoveAllListeners();
                    exitButton.onClick.AddListener(() => uiManager.OnExit());
                }
                else
                {
                    Debug.LogWarning("ExitButton no encontrado en LevelCompleteCanvas.");
                }
            }

            if (pauseCanvas != null)
            {
                Button continueButton = pauseCanvas.transform.Find("ContinueButton")?.GetComponent<Button>();
                if (continueButton != null)
                {
                    continueButton.onClick.RemoveAllListeners();
                    continueButton.onClick.AddListener(() => uiManager.OnContinue());
                }
                else
                {
                    Debug.LogWarning("ContinueButton no encontrado en PauseCanvas.");
                }
            }

            if (gameOverCanvas != null)
            {
                Button retryButton = gameOverCanvas.transform.Find("RetryButton")?.GetComponent<Button>();
                if (retryButton != null)
                {
                    retryButton.onClick.RemoveAllListeners();
                    retryButton.onClick.AddListener(() => uiManager.OnRetry());
                }
                else
                {
                    Debug.LogWarning("RetryButton no encontrado en GameOverCanvas.");
                }

                Button exitButton = gameOverCanvas.transform.Find("ExitButton")?.GetComponent<Button>();
                if (exitButton != null)
                {
                    exitButton.onClick.RemoveAllListeners();
                    exitButton.onClick.AddListener(() => uiManager.OnExit());
                }
                else
                {
                    Debug.LogWarning("ExitButton no encontrado en GameOverCanvas.");
                }
            }

            Button pauseButton = GameObject.Find("PauseButton")?.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(() => uiManager.OnPause());
                Debug.Log("PauseButton configurado correctamente en escena: " + gameObject.scene.name);
            }
            else
            {
                Debug.LogWarning("PauseButton no encontrado en la escena actual.");
            }
        }

        public void ShowPauseCanvas(bool show)
        {
            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(show);
            }
            else
            {
                Debug.LogWarning("PauseCanvas es null en ShowPauseCanvas.");
            }
        }

        public void ShowLevelCompleteCanvas(bool show)
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.SetActive(show);
            }
            else
            {
                Debug.LogWarning("LevelCompleteCanvas es null en ShowLevelCompleteCanvas.");
            }
        }

        public void ShowGameOverCanvas(bool show)
        {
            if (gameOverCanvas != null)
            {
                gameOverCanvas.SetActive(show);
            }
            else
            {
                Debug.LogWarning("GameOverCanvas es null en ShowGameOverCanvas.");
            }
        }
    }
}