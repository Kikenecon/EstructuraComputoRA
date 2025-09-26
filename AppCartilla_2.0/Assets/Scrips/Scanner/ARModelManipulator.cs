using UnityEngine;

public class ARModelManipulator : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveLerpSpeed = 9f; // velocidad de seguimiento al dedo

    [Header("Escala")]
    public float scaleSpeed = 0.001f;

    private Camera arCamera;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Plane dragPlane;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        // 🔹 Movimiento con un dedo
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragPlane = new Plane(arCamera.transform.forward * -1, transform.position);

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (dragPlane.Raycast(ray, out float enter))
                {
                    dragOffset = transform.position - ray.GetPoint(enter);
                }

                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 targetPos = ray.GetPoint(enter) + dragOffset;
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveLerpSpeed);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // 🔹 Escala con dos dedos (sin límites forzados)
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (prevTouch0 - prevTouch1).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            float scaleFactor = 1 + difference * scaleSpeed;

            transform.localScale *= scaleFactor;
        }
    }
}






//using UnityEngine;
//using UnityEngine.EventSystems;

//[DisallowMultipleComponent]
//public class ARModelManipulator : MonoBehaviour
//{
//    [Header("Referencias")]
//    public Camera arCamera; // si lo dejas null usa Camera.main
//    [Tooltip("Si no asignas este campo, el script buscará un TargetModelController en los padres.")]
//    public TargetModelController parentTarget;

//    [Header("Sensibilidades y límites")]
//    [Range(1f, 30f)] public float moveSmooth = 10f;        // mayor = más suave
//    public float rotationSpeed = 1.0f;                     // multiplicador de rotación
//    public float scaleSpeed = 1.0f;                        // multiplicador de escalado (ratio)
//    public float minScale = 0.1f;
//    public float maxScale = 3.0f;

//    // estado interno
//    private Transform modelT;
//    private Plane movePlane;
//    private bool isDragging = false;
//    private int dragFingerId = -1;
//    private Vector3 dragOffsetWorld = Vector3.zero;

//    void Start()
//    {
//        modelT = transform;
//        if (arCamera == null) arCamera = Camera.main;
//        if (parentTarget == null) parentTarget = GetComponentInParent<TargetModelController>();
//    }

//    // Llamar desde TargetModelController para habilitar/deshabilitar manipulación
//    public void SetActive(bool enabled)
//    {
//        enabledForManipulation = enabled;
//        if (!enabled) ResetInteractionFlags();
//    }

//    private bool enabledForManipulation = true;

//    void ResetInteractionFlags()
//    {
//        isDragging = false;
//        dragFingerId = -1;
//    }

//    void Update()
//    {
//        if (!enabledForManipulation) return;
//        if (parentTarget == null) return;
//        if (TargetModelController.CurrentActiveTarget != parentTarget) return;

//        // Si no hay dedos, limpiar flags
//        if (Input.touchCount == 0)
//        {
//            ResetInteractionFlags();
//            return;
//        }

//        if (Input.touchCount == 1)
//            HandleOneFinger(Input.GetTouch(0));
//        else if (Input.touchCount == 2)
//            HandleTwoFingers(Input.GetTouch(0), Input.GetTouch(1));
//    }

//    bool TouchOverUI(Touch t)
//    {
//        if (EventSystem.current == null) return false;
//        return EventSystem.current.IsPointerOverGameObject(t.fingerId);
//    }

//    void HandleOneFinger(Touch t)
//    {
//        if (TouchOverUI(t)) return;

//        if (t.phase == TouchPhase.Began)
//        {
//            // comenzar drag
//            isDragging = true;
//            dragFingerId = t.fingerId;

//            // plano paralelo al target (mueve sobre la "superficie" del target)
//            movePlane = new Plane(parentTarget.transform.up, modelT.position);

//            Ray r = arCamera.ScreenPointToRay(t.position);
//            if (movePlane.Raycast(r, out float enter))
//            {
//                Vector3 hitWorld = r.GetPoint(enter);
//                dragOffsetWorld = modelT.position - hitWorld;
//            }
//            else
//            {
//                dragOffsetWorld = Vector3.zero;
//            }
//        }
//        else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
//        {
//            if (!isDragging || t.fingerId != dragFingerId) return;

//            Ray r = arCamera.ScreenPointToRay(t.position);
//            if (movePlane.Raycast(r, out float enter))
//            {
//                Vector3 hitWorld = r.GetPoint(enter);
//                Vector3 targetPos = hitWorld + dragOffsetWorld;
//                // suavizado
//                modelT.position = Vector3.Lerp(modelT.position, targetPos, Time.deltaTime * moveSmooth);
//            }
//        }
//        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
//        {
//            if (t.fingerId == dragFingerId)
//            {
//                isDragging = false;
//                dragFingerId = -1;
//            }
//        }
//    }

//    void HandleTwoFingers(Touch t0, Touch t1)
//    {
//        if (TouchOverUI(t0) || TouchOverUI(t1)) return;

//        // --- ESCALA (pinch) usando ratio prevDist / currDist ---
//        Vector2 prevPos0 = t0.position - t0.deltaPosition;
//        Vector2 prevPos1 = t1.position - t1.deltaPosition;
//        float prevDist = (prevPos0 - prevPos1).magnitude;
//        float currDist = (t0.position - t1.position).magnitude;

//        if (prevDist > 0.0001f)
//        {
//            float ratio = currDist / prevDist;
//            // aplicar suavizado y límites
//            Vector3 targetScale = modelT.localScale * ratio * scaleSpeed;
//            float clamped = Mathf.Clamp(targetScale.x, minScale, maxScale);
//            Vector3 finalScale = Vector3.one * clamped;
//            modelT.localScale = Vector3.Lerp(modelT.localScale, finalScale, Time.deltaTime * moveSmooth);
//        }

//        // --- ROTACIÓN (giro con dos dedos) ---
//        Vector2 prevDir = prevPos1 - prevPos0;
//        Vector2 currDir = t1.position - t0.position;
//        if (prevDir.sqrMagnitude > 0.0001f && currDir.sqrMagnitude > 0.0001f)
//        {
//            float angle = Vector2.SignedAngle(prevDir, currDir);
//            // generar rotación objetivo alrededor del eje 'up' del target (comportamiento clásico AR)
//            Quaternion targetRot = Quaternion.AngleAxis(-angle * rotationSpeed, parentTarget.transform.up) * modelT.rotation;
//            modelT.rotation = Quaternion.Slerp(modelT.rotation, targetRot, Time.deltaTime * moveSmooth);
//        }

//        // Al usar dos dedos, cancelamos el drag de 1 dedo si estaba activo
//        isDragging = false;
//        dragFingerId = -1;
//    }
//}
