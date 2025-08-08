using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        PanelEvents.OnPanelManagerInitialized += ShowMainScreen;

        // Música de fondo para pantalla inicial
        AudioManager.Instance.PlayMusic(AudioManager.Instance.songFondo, true);
    }

    private void OnDisable()
    {
        PanelEvents.OnPanelManagerInitialized -= ShowMainScreen;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }
    }


    void ShowMainScreen()
    {
        PanelManager.Instance.ShowPanel("MainScreen");
    }
}
