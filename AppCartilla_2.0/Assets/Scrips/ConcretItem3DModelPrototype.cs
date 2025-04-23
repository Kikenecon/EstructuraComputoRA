using UnityEngine;

public class ConcretItem3DModelPrototype : MonoBehaviour, IPrototype
{
    public virtual GameObject Clone()
    {
        // Clonar el GameObject en la misma posición y rotación
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);

        // Obtener la escala global del original
        Vector3 originalGlobalScale = transform.lossyScale;

        // Desasignar el padre para que el clon sea independiente
        clone.transform.parent = null;

        // Ajustar la escala local del clon para que su escala global coincida con la del original
       
        clone.transform.localScale = originalGlobalScale;

        Debug.Log($"Escala original (lossyScale): {transform.lossyScale}, Escala del clon (localScale): {clone.transform.localScale}");

        return clone;

    }
}

