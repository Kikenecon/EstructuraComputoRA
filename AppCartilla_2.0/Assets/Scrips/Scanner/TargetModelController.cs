using UnityEngine;
using Vuforia;

public class TargetModelController : MonoBehaviour
{
    [Header("Model root (si no pones nada se usará el primer hijo)")]
    public GameObject modelRoot;

    [Tooltip("Si true, cuando el target se pierde, el modelo se ocultará (comportamiento Vuforia-like).")]
    public bool hideOnLost = true;

    private ObserverBehaviour observerBehaviour;
    private ARModelManipulator manipulator;

    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;
    private Vector3 defaultLocalScale;

    // Último target detectado
    public static TargetModelController CurrentActiveTarget { get; private set; }

    void Start()
    {
        if (modelRoot == null && transform.childCount > 0)
            modelRoot = transform.GetChild(0).gameObject;

        if (modelRoot != null)
        {
            defaultLocalPosition = modelRoot.transform.localPosition;
            defaultLocalRotation = modelRoot.transform.localRotation;
            defaultLocalScale = modelRoot.transform.localScale;
            manipulator = modelRoot.GetComponent<ARModelManipulator>();
        }

        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDestroy()
    {
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool found = status.Status == Status.TRACKED ||
                     status.Status == Status.EXTENDED_TRACKED;

        if (found) OnFound();
        else OnLost();
    }

    private void OnFound()
    {
        Debug.Log($"[TargetModelController] OnFound: {gameObject.name}");
        if (modelRoot != null)
        {
            ResetTransformToDefault();
            modelRoot.SetActive(true);

            if (manipulator != null)
                manipulator.gameObject.SetActive(true);
        }
        CurrentActiveTarget = this;
    }

    private void OnLost()
    {
        Debug.Log($"[TargetModelController] OnLost: {gameObject.name}");
        if (manipulator != null)
            manipulator.gameObject.SetActive(false);

        if (hideOnLost && modelRoot != null)
            modelRoot.SetActive(false);

        if (CurrentActiveTarget == this)
            CurrentActiveTarget = null;
    }

    // ===== Métodos públicos usados por el botón =====

    // Oculta el modelo (invocado por el botón)
    public void HideModel()
    {
        Debug.Log($"[TargetModelController] HideModel called: {gameObject.name}");
        if (modelRoot != null)
        {
            modelRoot.SetActive(false);
            if (manipulator != null)
                manipulator.gameObject.SetActive(false);
        }

        if (CurrentActiveTarget == this)
            CurrentActiveTarget = null;
    }

    // Restaurar (usa el transform por defecto). Puede ser llamado si quieres forzar la reaparición.
    public void RestoreFromUser()
    {
        Debug.Log($"[TargetModelController] RestoreFromUser called: {gameObject.name}");
        if (modelRoot != null)
        {
            ResetTransformToDefault();
            modelRoot.SetActive(true);

            if (manipulator != null)
                manipulator.gameObject.SetActive(true);

            CurrentActiveTarget = this;
        }
    }

    // Restaura transform local guardado en Start()
    public void ResetTransformToDefault()
    {
        if (modelRoot == null) return;

        modelRoot.transform.localPosition = defaultLocalPosition;
        modelRoot.transform.localRotation = defaultLocalRotation;
        modelRoot.transform.localScale = defaultLocalScale;
    }

    public bool IsModelActive()
    {
        return modelRoot != null && modelRoot.activeSelf;
    }
}
