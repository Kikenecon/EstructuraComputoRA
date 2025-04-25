using UnityEngine; 

namespace AssemblyGame
{
    public class PointsFault : Fault
    {
        private int pointsToDeduct = 5;

        public override void ApplyEffect(AssemblyGameManager gameManager)
        {
            gameManager.AddScore(-pointsToDeduct);
            Debug.Log($"PointsFault ha alcanzado un slot. Se restaron {pointsToDeduct} puntos.");
        }
    }
}
