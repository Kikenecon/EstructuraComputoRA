using UnityEngine;

namespace AssemblyGame
{
    public class TimeFault : Fault
    {
        private float timeToDeduct = 5f;

        public override void ApplyEffect(AssemblyGameManager gameManager)
        {
            gameManager.DeductTime(timeToDeduct);
            Debug.Log($"TimeFault ha alcanzado un slot. Se restaron {timeToDeduct} segundos.");
        }
    }
}
