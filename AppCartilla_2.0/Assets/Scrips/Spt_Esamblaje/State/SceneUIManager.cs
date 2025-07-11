using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AssemblyGame
{
    public class SceneUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI nextPartText;
        [SerializeField] private GameObject pauseButtonObject;

        private AssemblyGameManager gameManager;
        private bool isPaused = false;

        private void Awake()
        {
            gameManager = AssemblyGameManager.getInstance();
            if (gameManager == null)
            {
                Debug.LogError("AssemblyGameManager no encontrado. Asegúrate de que exista en Level1Scene.");
                return;
            }

            if (pausePanel != null) pausePanel.SetActive(false);
            else Debug.LogWarning("PausePanel no asignado en SceneUIManager.");
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            else Debug.LogWarning("LevelCompletePanel no asignado en SceneUIManager.");
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            else Debug.LogWarning("GameOverPanel no asignado en SceneUIManager.");
            if (nextPartText != null) nextPartText.gameObject.SetActive(true);
            else Debug.LogWarning("NextPartText no asignado en SceneUIManager.");

            if (pauseButtonObject != null)
            {
                Button pauseButton = pauseButtonObject.GetComponent<Button>();
                if (pauseButton == null)
                {
                    Debug.LogError("pauseButtonObject no tiene un componente Button.");
                }
            }
            else
            {
                Debug.LogWarning("pauseButtonObject no asignado en SceneUIManager.");
            }
        }

        public void OnPause()
        {
            if (isPaused) return;
            isPaused = true;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                gameManager.SetState(new PausedState());
                Time.timeScale = 0f;
                Debug.Log("PausePanel activado.");
            }
            else
            {
                Debug.LogError("PausePanel no está asignado.");
            }
        }

        public void OnContinue()
        {
            if (!isPaused) return;
            isPaused = false;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f;
                gameManager.SetState(new PlayingState());
                Debug.Log("PausePanel desactivado.");
            }
            else
            {
                Debug.LogError("PausePanel no está asignado.");
            }
        }

        public void OnNextLevel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
                Time.timeScale = 1f;
                gameManager.AdvanceToNextLevel();
                Debug.Log("NextLevel activado.");
            }
        }

        public void OnRetry()
        {
            if (levelCompletePanel != null || gameOverPanel != null)
            {
                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                Time.timeScale = 1f;
                gameManager.ResetGame();
                SceneManager.LoadScene("Level1Scene");
                Debug.Log("Retry activado.");
            }
        }

        public void OnExit()
        {
            if (levelCompletePanel != null || gameOverPanel != null)
            {
                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                Time.timeScale = 1f;
                gameManager.ResetGame(); // Reinicia todos los valores
                SceneManager.LoadScene("Menu_Ensamblaje"); // Carga el menú
                Debug.Log("Juego reiniciado y enviado a Menu_Ensamblaje.");
            }
        }

        public void ShowLevelCompletePanel(bool show)
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(show);
                if (show) Time.timeScale = 0f;
                else Time.timeScale = 1f;
                Debug.Log($"ShowLevelCompletePanel: {show}");
            }
            else
            {
                Debug.LogError("levelCompletePanel no está asignado.");
            }
        }

        public void ShowGameOverPanel(bool show)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(show);
                if (show) Time.timeScale = 0f;
                else Time.timeScale = 1f;
                Debug.Log($"ShowGameOverPanel: {show}");
            }
            else
            {
                Debug.LogError("gameOverPanel no está asignado.");
            }
        }
    }
}

//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//namespace AssemblyGame
//{
//    public class SceneUIManager : MonoBehaviour
//    {
//        [SerializeField] private GameObject pausePanel;
//        [SerializeField] private GameObject levelCompletePanel;
//        [SerializeField] private GameObject gameOverPanel;
//        [SerializeField] private TextMeshProUGUI nextPartText;
//        [SerializeField] private GameObject pauseButtonObject;

//        private AssemblyGameManager gameManager;
//        private bool isPaused = false;

//        private void Awake()
//        {
//            gameManager = AssemblyGameManager.getInstance();
//            if (gameManager == null)
//            {
//                Debug.LogError("AssemblyGameManager no encontrado. Asegúrate de que exista en Level1Scene.");
//                return;
//            }

//            if (pausePanel != null) pausePanel.SetActive(false);
//            else Debug.LogWarning("PausePanel no asignado en SceneUIManager.");
//            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
//            else Debug.LogWarning("LevelCompletePanel no asignado en SceneUIManager.");
//            if (gameOverPanel != null) gameOverPanel.SetActive(false);
//            else Debug.LogWarning("GameOverPanel no asignado en SceneUIManager.");
//            if (nextPartText != null) nextPartText.gameObject.SetActive(true);
//            else Debug.LogWarning("NextPartText no asignado en SceneUIManager.");

//            if (pauseButtonObject != null)
//            {
//                Button pauseButton = pauseButtonObject.GetComponent<Button>();
//                if (pauseButton == null)
//                {
//                    Debug.LogError("pauseButtonObject no tiene un componente Button.");
//                }
//            }
//            else
//            {
//                Debug.LogWarning("pauseButtonObject no asignado en SceneUIManager.");
//            }
//        }

//        public void OnPause()
//        {
//            if (isPaused) return;
//            isPaused = true;
//            if (pausePanel != null)
//            {
//                pausePanel.SetActive(true);
//                gameManager.SetState(new PausedState());
//                Time.timeScale = 0f;
//                Debug.Log("PausePanel activado.");
//            }
//            else
//            {
//                Debug.LogError("PausePanel no está asignado.");
//            }
//        }

//        public void OnContinue()
//        {
//            if (!isPaused) return;
//            isPaused = false;
//            if (pausePanel != null)
//            {
//                pausePanel.SetActive(false);
//                Time.timeScale = 1f;
//                gameManager.SetState(new PlayingState());
//                Debug.Log("PausePanel desactivado.");
//            }
//            else
//            {
//                Debug.LogError("PausePanel no está asignado.");
//            }
//        }

//        public void OnNextLevel()
//        {
//            if (levelCompletePanel != null)
//            {
//                levelCompletePanel.SetActive(false);
//                Time.timeScale = 1f;
//                gameManager.AdvanceToNextLevel();
//                Debug.Log("NextLevel activado.");
//            }
//        }

//        public void OnRetry()
//        {
//            if (levelCompletePanel != null || gameOverPanel != null)
//            {
//                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
//                if (gameOverPanel != null) gameOverPanel.SetActive(false);
//                Time.timeScale = 1f;
//                gameManager.ResetGame();
//                Debug.Log("Retry activado.");
//            }
//        }

//        public void OnExit()
//        {
//            if (levelCompletePanel != null || gameOverPanel != null)
//            {
//                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
//                if (gameOverPanel != null) gameOverPanel.SetActive(false);
//                Time.timeScale = 1f;
//                Application.Quit();
//#if UNITY_EDITOR
//                UnityEditor.EditorApplication.isPlaying = false;
//#endif
//                Debug.Log("Exit activado.");
//            }
//        }

//        public void ShowLevelCompletePanel(bool show)
//        {
//            if (levelCompletePanel != null)
//            {
//                levelCompletePanel.SetActive(show);
//                if (show) Time.timeScale = 0f;
//                else Time.timeScale = 1f;
//                Debug.Log($"ShowLevelCompletePanel: {show}");
//            }
//            else
//            {
//                Debug.LogError("levelCompletePanel no está asignado.");
//            }
//        }

//        public void ShowGameOverPanel(bool show)
//        {
//            if (gameOverPanel != null)
//            {
//                gameOverPanel.SetActive(show);
//                if (show) Time.timeScale = 0f;
//                else Time.timeScale = 1f;
//                Debug.Log($"ShowGameOverPanel: {show}");
//            }
//            else
//            {
//                Debug.LogError("gameOverPanel no está asignado.");
//            }
//        }
//    }
//}