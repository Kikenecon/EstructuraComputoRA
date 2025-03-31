using UnityEngine;

public class Item3DModelPrototype : MonoBehaviour, IPrototype
{
    public GameObject Clone()
    {
        // Clonar el GameObject en la misma posición y rotación
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);

        // Obtener la escala global del original
        Vector3 originalGlobalScale = transform.lossyScale;

        // Desasignar el padre para que el clon sea independiente
        clone.transform.parent = null;

        // Ajustar la escala local del clon para que su escala global coincida con la del original
        // Dado que el clon ya no tiene padre, su localScale será igual a su lossyScale
        clone.transform.localScale = originalGlobalScale;

        Debug.Log($"Escala original (lossyScale): {transform.lossyScale}, Escala del clon (localScale): {clone.transform.localScale}");

        return clone;

    }
}



//using UnityEngine;

//public class Item3DModelPrototype : MonoBehaviour, IPrototype
//{
//    public GameObject Clone()
//    {
//        // Usamos Instantiate para clonar el GameObject
//        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
//        clone.transform.localScale = transform.localScale;
//        return clone;
//    }
//}
