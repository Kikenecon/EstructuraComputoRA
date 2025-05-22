using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubclasslItem3DModelPrototype : ConcretItem3DModelPrototype
{
    
    [SerializeField] private float rotationSpeed = 30f;

    public override GameObject Clone()
    {
        // Llamar al método Clone de la clase base para copiar posición, rotación y escala
        GameObject clone = base.Clone();

        // Añadir el componente RotateEffect al clon
        RotateEffect rotateEffect = clone.AddComponent<RotateEffect>();
        rotateEffect.SetRotationSpeed(rotationSpeed);

        return clone;
    }
}
