using UnityEngine;
using UnityEngine.UI;

public class ModelDeleteButton : MonoBehaviour
{
    [Header("Opcional - referencia al Button (se autoconfigura si está en el mismo GameObject)")]
    public Button button;
    public Text labelText;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnDeleteButtonPressed);
        }
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnDeleteButtonPressed);
    }

    void Update()
    {
        // Mostrar estado en el label (si tienes uno) y activar/desactivar interactividad
        var t = TargetModelController.CurrentActiveTarget;
        if (button == null) return;

        if (t == null)
        {
            button.interactable = false;
            if (labelText != null) labelText.text = "No target";
        }
        else
        {
            button.interactable = true;
            if (labelText != null) labelText.text = t.IsModelActive() ? "Eliminar" : "Restaurar";
        }
    }

    // Método público conectado al OnClick del Button
    public void OnDeleteButtonPressed()
    {
        var t = TargetModelController.CurrentActiveTarget;
        if (t == null)
        {
            Debug.Log("[ModelDeleteButton] No hay target activo al presionar el botón.");
            return;
        }

        if (t.IsModelActive())
        {
            Debug.Log($"[ModelDeleteButton] Ocultando modelo del target {t.gameObject.name}");
            t.HideModel();
        }
        else
        {
            Debug.Log($"[ModelDeleteButton] Restaurando modelo del target {t.gameObject.name}");
            t.RestoreFromUser();
        }
    }
}
