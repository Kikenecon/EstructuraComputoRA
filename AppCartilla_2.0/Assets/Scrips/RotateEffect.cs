using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación en grados por segundo

    void Update()
    {
        // Rotar el objeto alrededor de su eje Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
}
