using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Nombre del Panel de Pausa en PanelManager")]
    public string pausePanelId = "PauseScreen";

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        // Mostrar panel de pausa
        PanelManager.Instance.ShowPanel(pausePanelId, PanelShowBehaviour.HIDE_PREVIOUS);

        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Ocultar panel de pausa (volvemos al anterior)
        PanelManager.Instance.HideLastPanel();

       
    }
}
