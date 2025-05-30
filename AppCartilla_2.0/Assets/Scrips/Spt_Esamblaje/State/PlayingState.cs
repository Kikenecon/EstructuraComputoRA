using UnityEngine;

namespace AssemblyGame
{
    public class PlayingState : IGameState
    {
        private float faultSpawnTimer = 0f;
        private float faultSpawnInterval = 5f;

        public void OnEnter(AssemblyGameManager context)
        {
            Debug.Log("Entering Playing State...");
            Time.timeScale = 1f;
            faultSpawnTimer = 0f;
        }

        public void OnExit(AssemblyGameManager context)
        {
            Debug.Log("Exiting Playing State...");
        }

        public void Update(AssemblyGameManager context)
        {
            if (context.GetCurrentLevel() >= 2)
            {
                faultSpawnTimer += Time.deltaTime;
                if (faultSpawnTimer >= faultSpawnInterval)
                {
                    context.SpawnFault();
                    faultSpawnTimer = 0f;
                }
            }
            context.DeductTime(Time.deltaTime);
        }

        public void HandleInput(AssemblyGameManager context)
        {
        }
    }
}