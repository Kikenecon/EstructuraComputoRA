


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