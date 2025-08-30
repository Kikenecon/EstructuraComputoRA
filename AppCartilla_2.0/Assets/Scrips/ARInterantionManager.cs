using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARInterantionManager : MonoBehaviour
{
    [SerializeField] private Camera aRCamera;
    [SerializeField] private float previewDistance = 1f; // Distancia configurable para el preview (ajusta en Inspector)
    private GameObject aRPointer;
    private GameObject item3DModel;
    private GameObject itemSelected;
    private bool isOverUI;
    private bool isOver3DModel;
    private Vector2 initialTouchPOS;

    [SerializeField] private float speedMovement = 14.0f;
    [SerializeField] private float speedRotation = 15.0f;
    [SerializeField] private float scaleFactor = 6.0f;

    private float screenFactor = 0.0001f;
    private float touchDis;
    private Vector2 touchPositionDiff;
    private float rotationTolerance = 1.5f;
    private float scaleTolerance = 20f;

    public GameObject Item3DModel
    {
        set
        {
            item3DModel = value;
            item3DModel.transform.position = aRPointer.transform.position;
            item3DModel.transform.parent = aRPointer.transform;
            // Hacer que el modelo mire hacia la cámara (su forward hacia el usuario)
            Vector3 directionToCamera = aRCamera.transform.position - item3DModel.transform.position;
            item3DModel.transform.rotation = Quaternion.LookRotation(-directionToCamera, Vector3.up);
            aRPointer.SetActive(true); // Asegurarse de que el puntero esté activo
        }
    }

    void Start()
    {
        aRPointer = transform.GetChild(0).gameObject;
        // Hacer aRPointer hijo de la cámara AR para que siga su movimiento
        aRPointer.transform.SetParent(aRCamera.transform);
        aRPointer.transform.localPosition = new Vector3(0, 0, previewDistance);
        aRPointer.transform.localRotation = Quaternion.identity;
        aRPointer.SetActive(false); // Desactivar inicialmente

        GameManager.instance.OnMainMenu += SetItemPosition;
    }

    private void SetItemPosition()
    {
        if (item3DModel != null)
        {
            item3DModel.transform.parent = null;
            aRPointer.SetActive(false);
            item3DModel = null;
        }
    }

    public void DeleteItem()
    {
        Destroy(item3DModel);
        aRPointer.SetActive(false);
        GameManager.instance.MainMenu();
    }

    public void DuplicateItem()
    {
        if (item3DModel != null)
        {
            // Obtener el componente IPrototype del modelo 3D
            IPrototype prototype = item3DModel.GetComponent<IPrototype>();
            if (prototype != null)
            {
                // Clonar el modelo usando el patrón Prototype
                GameObject clonedItem = prototype.Clone();
            }
            else
            {
                Debug.LogWarning("El modelo 3D no tiene el componente IPrototype.");
            }
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touchOne = Input.GetTouch(0);
            if (touchOne.phase == TouchPhase.Began)
            {
                initialTouchPOS = touchOne.position;
                isOverUI = isTapOverUI(initialTouchPOS);
                isOver3DModel = isTapOver3DModel(initialTouchPOS);
            }

            if (touchOne.phase == TouchPhase.Moved && isOver3DModel)
            {
                Vector2 diffPos = (touchOne.position - initialTouchPOS) * screenFactor;
                // Movimiento relativo a la cámara: derecha/izquierda y arriba/abajo en pantalla
                Vector3 moveDirection = (aRCamera.transform.right * diffPos.x + aRCamera.transform.up * diffPos.y) * speedMovement;
                item3DModel.transform.position += moveDirection;
                initialTouchPOS = touchOne.position;
            }

            if (Input.touchCount == 2)
            {
                Touch touchTwo = Input.GetTouch(1);
                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
                {
                    touchPositionDiff = touchTwo.position - touchOne.position;
                    touchDis = Vector2.Distance(touchTwo.position, touchOne.position);
                }

                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPosDiff = touchTwo.position - touchOne.position;
                    float currentTouchDis = Vector2.Distance(touchTwo.position, touchOne.position);
                    float diffDis = currentTouchDis - touchDis;

                    if (Mathf.Abs(diffDis) > scaleTolerance)
                    {
                        Vector3 newScale = item3DModel.transform.localScale + Mathf.Sign(diffDis) * Vector3.one * scaleFactor;
                        item3DModel.transform.localScale = Vector3.Lerp(item3DModel.transform.localScale, newScale, 0.05f);
                    }

                    float angle = Vector2.SignedAngle(touchPositionDiff, currentTouchPosDiff);

                    if (Mathf.Abs(angle) > rotationTolerance)
                    {
                        item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.rotation.eulerAngles.y - Mathf.Sign(angle) * speedRotation, 0);
                    }

                    touchDis = currentTouchDis;
                    touchPositionDiff = currentTouchPosDiff;
                }
            }

            if (isOver3DModel && item3DModel == null && !isOverUI)
            {
                GameManager.instance.ARPosition();
                item3DModel = itemSelected;
                itemSelected = null;
                aRPointer.SetActive(true);
                item3DModel.transform.position = aRPointer.transform.position;
                item3DModel.transform.rotation = Quaternion.LookRotation(-(aRCamera.transform.position - item3DModel.transform.position), Vector3.up);
                item3DModel.transform.parent = aRPointer.transform;
            }
        }
    }

    private bool isTapOver3DModel(Vector2 touchPosition)
    {
        Ray ray = aRCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit3DModel))
        {
            if (hit3DModel.collider.CompareTag("item"))
            {
                itemSelected = hit3DModel.transform.gameObject;
                return true;
            }
        }
        return false;
    }

    private bool isTapOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);

        return result.Count > 0;
    }
}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class ARInterantionManager : MonoBehaviour
//{
//    [SerializeField] private Camera aRCamera;
//    [SerializeField] private float previewDistance = 1f; // Distancia configurable para el preview (ajusta en Inspector)
//    private GameObject aRPointer;
//    private GameObject item3DModel;
//    private GameObject itemSelected;
//    private bool isOverUI;
//    private bool isOver3DModel;
//    private Vector2 initialTouchPOS;

//    [SerializeField] private float speedMovement = 14.0f;
//    [SerializeField] private float speedRotation = 15.0f;
//    [SerializeField] private float scaleFactor = 6.0f;

//    private float screenFactor = 0.0001f;
//    private float touchDis;
//    private Vector2 touchPositionDiff;
//    private float rotationTolerance = 1.5f;
//    private float scaleTolerance = 20f;

//    public GameObject Item3DModel
//    {
//        set
//        {
//            item3DModel = value;
//            item3DModel.transform.position = aRPointer.transform.position;
//            item3DModel.transform.parent = aRPointer.transform;
//            aRPointer.SetActive(true); // Asegurarse de que el puntero esté activo
//        }
//    }

//    void Start()
//    {
//        aRPointer = transform.GetChild(0).gameObject;
//        // Hacer aRPointer hijo de la cámara AR para que siga su movimiento
//        aRPointer.transform.SetParent(aRCamera.transform);
//        aRPointer.transform.localPosition = new Vector3(0, 0, previewDistance);
//        aRPointer.transform.localRotation = Quaternion.identity;
//        aRPointer.SetActive(false); // Desactivar inicialmente

//        GameManager.instance.OnMainMenu += SetItemPosition;
//    }

//    private void SetItemPosition()
//    {
//        if (item3DModel != null)
//        {
//            item3DModel.transform.parent = null;
//            aRPointer.SetActive(false);
//            item3DModel = null;
//        }
//    }

//    public void DeleteItem()
//    {
//        Destroy(item3DModel);
//        aRPointer.SetActive(false);
//        GameManager.instance.MainMenu();
//    }

//    public void DuplicateItem()
//    {
//        if (item3DModel != null)
//        {
//            // Obtener el componente IPrototype del modelo 3D
//            IPrototype prototype = item3DModel.GetComponent<IPrototype>();
//            if (prototype != null)
//            {
//                // Clonar el modelo usando el patrón Prototype
//                GameObject clonedItem = prototype.Clone();
//            }
//            else
//            {
//                Debug.LogWarning("El modelo 3D no tiene el componente IPrototype.");
//            }
//        }
//    }

//    void Update()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touchOne = Input.GetTouch(0);
//            if (touchOne.phase == TouchPhase.Began)
//            {
//                initialTouchPOS = touchOne.position;
//                isOverUI = isTapOverUI(initialTouchPOS);
//                isOver3DModel = isTapOver3DModel(initialTouchPOS);
//            }

//            if (touchOne.phase == TouchPhase.Moved && isOver3DModel)
//            {
//                Vector2 diffPos = (touchOne.position - initialTouchPOS) * screenFactor;
//                // Movimiento relativo a la cámara: derecha/izquierda y arriba/abajo en pantalla
//                Vector3 moveDirection = (aRCamera.transform.right * diffPos.x + aRCamera.transform.up * diffPos.y) * speedMovement;
//                item3DModel.transform.position += moveDirection;
//                initialTouchPOS = touchOne.position;
//            }

//            if (Input.touchCount == 2)
//            {
//                Touch touchTwo = Input.GetTouch(1);
//                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
//                {
//                    touchPositionDiff = touchTwo.position - touchOne.position;
//                    touchDis = Vector2.Distance(touchTwo.position, touchOne.position);
//                }

//                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
//                {
//                    Vector2 currentTouchPosDiff = touchTwo.position - touchOne.position;
//                    float currentTouchDis = Vector2.Distance(touchTwo.position, touchOne.position);
//                    float diffDis = currentTouchDis - touchDis;

//                    if (Mathf.Abs(diffDis) > scaleTolerance)
//                    {
//                        Vector3 newScale = item3DModel.transform.localScale + Mathf.Sign(diffDis) * Vector3.one * scaleFactor;
//                        item3DModel.transform.localScale = Vector3.Lerp(item3DModel.transform.localScale, newScale, 0.05f);
//                    }

//                    float angle = Vector2.SignedAngle(touchPositionDiff, currentTouchPosDiff);

//                    if (Mathf.Abs(angle) > rotationTolerance)
//                    {
//                        item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.rotation.eulerAngles.y - Mathf.Sign(angle) * speedRotation, 0);
//                    }

//                    touchDis = currentTouchDis;
//                    touchPositionDiff = currentTouchPosDiff;
//                }
//            }

//            if (isOver3DModel && item3DModel == null && !isOverUI)
//            {
//                GameManager.instance.ARPosition();
//                item3DModel = itemSelected;
//                itemSelected = null;
//                aRPointer.SetActive(true);
//                item3DModel.transform.position = aRPointer.transform.position;
//                item3DModel.transform.parent = aRPointer.transform;
//            }
//        }
//    }

//    private bool isTapOver3DModel(Vector2 touchPosition)
//    {
//        Ray ray = aRCamera.ScreenPointToRay(touchPosition);
//        if (Physics.Raycast(ray, out RaycastHit hit3DModel))
//        {
//            if (hit3DModel.collider.CompareTag("item"))
//            {
//                itemSelected = hit3DModel.transform.gameObject;
//                return true;
//            }
//        }
//        return false;
//    }

//    private bool isTapOverUI(Vector2 touchPosition)
//    {
//        PointerEventData eventData = new PointerEventData(EventSystem.current);
//        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

//        List<RaycastResult> result = new List<RaycastResult>();
//        EventSystem.current.RaycastAll(eventData, result);

//        return result.Count > 0;
//    }
//}

