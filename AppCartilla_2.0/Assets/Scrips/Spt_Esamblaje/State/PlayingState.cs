using UnityEngine;
using UnityEngine.SceneManagement;

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
            faultSpawnTimer = 0f;
        }

        public void Update(AssemblyGameManager context)
        {
            if (context.GetCurrentLevel() >= 2 &&
                (SceneManager.GetActiveScene().name == "Level2Scene" || SceneManager.GetActiveScene().name == "Level3Scene"))
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

        public void ResetFaultTimer()
        {
            faultSpawnTimer = 0f;
            Debug.Log("FaultSpawnTimer reseteado a 0 en PlayingState.");
        }
    }
}//using UnityEngine;
 //using UnityEngine.SceneManagement;

//namespace AssemblyGame
//{
//    public class PlayingState : IGameState
//    {
//        private float faultSpawnTimer = 0f;
//        private float faultSpawnInterval = 5f;

//        public void OnEnter(AssemblyGameManager context)
//        {
//            Debug.Log("Entering Playing State...");
//            Time.timeScale = 1f;
//            faultSpawnTimer = 0f;
//        }

//        public void OnExit(AssemblyGameManager context)
//        {
//            Debug.Log("Exiting Playing State...");
//            faultSpawnTimer = 0f; // Resetea al salir por seguridad
//        }

//        public void Update(AssemblyGameManager context)
//        {
//            if (context.GetCurrentLevel() >= 2 &&
//                (SceneManager.GetActiveScene().name == "Level2Scene" || SceneManager.GetActiveScene().name == "Level3Scene"))
//            {
//                faultSpawnTimer += Time.deltaTime;
//                if (faultSpawnTimer >= faultSpawnInterval)
//                {
//                    context.SpawnFault();
//                    faultSpawnTimer = 0f;
//                }
//            }
//            context.DeductTime(Time.deltaTime);
//        }

//        public void HandleInput(AssemblyGameManager context)
//        {
//        }

//        Método añadido para resetear el temporizador desde AssemblyGameManager
//        public void ResetFaultTimer()
//        {
//            faultSpawnTimer = 0f;
//            Debug.Log("FaultSpawnTimer reseteado a 0 en PlayingState.");
//        }
//    }
//}