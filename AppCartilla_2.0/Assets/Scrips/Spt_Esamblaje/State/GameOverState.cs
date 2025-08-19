//using UnityEngine;

//namespace AssemblyGame
//{
//    public class GameOverState : IGameState
//    {
//        private float delayBeforeRestart = 3f; private float timer = 0f;
//        public void OnEnter(AssemblyGameManager context)
//        {
//            Debug.Log("Entering Game Over State");
//            context.SetGameOverTextActive(true);
//        }

//        public void OnExit(AssemblyGameManager context)
//        {
//            Debug.Log("Exiting Game Over State");
//            context.SetGameOverTextActive(false);
//        }

//        public void Update(AssemblyGameManager context)
//        {
//            timer += Time.deltaTime;
//            if (timer >= delayBeforeRestart)
//            {
//                context.ResetGame();
//                context.SetState(new PlayingState());
//            }
//        }

//        public void HandleInput(AssemblyGameManager context)
//        {
//            // Podríamos añadir opciones como "Reintentar" si tenemos botones
//        }
//    }
//}


using UnityEngine;

namespace AssemblyGame
{
    public class GameOverState : IGameState
    {
        public void OnEnter(AssemblyGameManager context)
        {
            Debug.Log("Entering Game Over State");
            Time.timeScale = 0f; // Pausar el juego
            SceneUIManager uiManager = Object.FindObjectOfType<SceneUIManager>(); // Usar el nombre completo como prueba
            if (uiManager != null)
            {
                uiManager.ShowGameOverPanel(true); // Usar el método de SceneUIManager
            }
            else
            {
                Debug.LogError("SceneUIManager no encontrado en la escena actual.");
            }
        }

        public void OnExit(AssemblyGameManager context)
        {
            Debug.Log("Exiting Game Over State");
            Time.timeScale = 1f; // Reanudar el tiempo al salir
        }

        public void Update(AssemblyGameManager context)
        {
            // No se necesita lógica de actualización, los botones manejan las acciones
        }

        public void HandleInput(AssemblyGameManager context)
        {
            // Los botones en el GameOverCanvas manejan las interacciones
        }
    }
}