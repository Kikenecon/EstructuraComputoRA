using UnityEngine;

namespace AssemblyGame
{
    public interface IFault
    {
        void MoveTowardsTarget(Vector2 targetPosition);
        void ApplyEffect(AssemblyGameManager gameManager);
        void OnDestroy();
    }
}
