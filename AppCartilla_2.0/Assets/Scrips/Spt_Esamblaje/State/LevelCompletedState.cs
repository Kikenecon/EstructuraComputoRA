using UnityEngine;

namespace AssemblyGame
{
    public class LevelCompletedState : IGameState
    {
        public void OnEnter(AssemblyGameManager context)
        {
            Debug.Log("Entering LevelCompleted State...");
            Time.timeScale = 0f; // Pausar el juego
        }

        public void OnExit(AssemblyGameManager context)
        {
            Debug.Log("Exiting LevelCompleted State...");
            Time.timeScale = 1f; // Reanudar el juego
        }

        public void Update(AssemblyGameManager context)
        {
            // No se necesita actualización mientras se muestra el canvas
        }

        public void HandleInput(AssemblyGameManager context)
        {
            // Las acciones (como NextLevel, Retry, Exit) se manejan mediante los botones del canvas
        }
    }
}