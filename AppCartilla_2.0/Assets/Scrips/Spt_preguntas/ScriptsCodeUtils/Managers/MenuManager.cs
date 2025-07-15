using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        PanelEvents.OnPanelManagerInitialized += ShowMainScreen;
    }

    private void OnDisable()
    {
        PanelEvents.OnPanelManagerInitialized -= ShowMainScreen;
    }

    void ShowMainScreen()
    {
        PanelManager.Instance.ShowPanel("MainScreen");
    }
}