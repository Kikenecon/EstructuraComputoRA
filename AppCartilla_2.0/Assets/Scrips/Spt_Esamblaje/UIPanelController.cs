using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    [Header("Panel que se va a mostrar/ocultar")]
    [SerializeField] private GameObject panel;

    /// <summary>
    /// Activa el panel asignado.
    /// </summary>
    public void MostrarPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[UIPanelController] No se asignó un panel en el objeto '{gameObject.name}'.");
        }
    }

    /// <summary>
    /// Desactiva el panel asignado.
    /// </summary>
    public void OcultarPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[UIPanelController] No se asignó un panel en el objeto '{gameObject.name}'.");
        }
    }
}
