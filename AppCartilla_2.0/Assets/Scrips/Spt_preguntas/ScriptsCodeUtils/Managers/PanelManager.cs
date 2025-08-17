using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    private List<PanelInstanceModel> _panelInstanceModels = new List<PanelInstanceModel>();
    private ObjectPool _objectPool;

    private void Start()
    {
        _objectPool = ObjectPool.Instance;
        PanelEvents.OnPanelManagerInitialized?.Invoke();
    }

    public void ShowPanel(string panelId, PanelShowBehaviour behaviour = PanelShowBehaviour.KEEP_PREVIOUS)
    {
        GameObject panelInstance = _objectPool.GetObjectFromPool(panelId);

        if (panelInstance != null)
        {
            if (behaviour == PanelShowBehaviour.HIDE_PREVIOUS && GetAmountPanelsInQueue() > 0)
            {
                var lastPanel = GetLastPanel();
                lastPanel?.PanelInstance.SetActive(false);
            }

            _panelInstanceModels.Add(new PanelInstanceModel
            {
                PanelId = panelId,
                PanelInstance = panelInstance
            });

            PanelEvents.OnPanelShown?.Invoke(panelId);

            // 🎵 Reproducir audio según panel
            switch (panelId)
            {
                case "MainScreen":
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.songFondo, true);
                    break;

                case "WheelScreen":
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.songFondo, true);
                    break;

                case "QuestionScreen":
                    // Aquí reproduciremos correct/incorrect en otra parte del código cuando se acierte/falle
                    AudioManager.Instance.StopMusic();
                    break;

                case "FinalScreen":
                    AudioManager.Instance.StopMusic();
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Trying to use panelId = {panelId}, but this is not found in the ObjectPool");
        }
    }

    public void HideLastPanel()
    {
        if (AnyPanelShowing())
        {
            var lastPanel = GetLastPanel();
            _panelInstanceModels.Remove(lastPanel);
            _objectPool.PoolObject(lastPanel.PanelInstance);

            if (GetAmountPanelsInQueue() > 0)
            {
                lastPanel = GetLastPanel();
                if (lastPanel != null && !lastPanel.PanelInstance.activeInHierarchy)
                {
                    lastPanel.PanelInstance.SetActive(true);
                }
            }

            PanelEvents.OnPanelHidden?.Invoke();
        }
    }

    PanelInstanceModel GetLastPanel()
    {
        return _panelInstanceModels[_panelInstanceModels.Count - 1];
    }

    public bool AnyPanelShowing()
    {
        return GetAmountPanelsInQueue() > 0;
    }

    public int GetAmountPanelsInQueue()
    {
        return _panelInstanceModels.Count;
    }
}
