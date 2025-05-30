using UnityEngine;

namespace AssemblyGame
{
    public class PausedState : IGameState
    {
        public void OnEnter(AssemblyGameManager context)
        {
            // No hacemos nada al entrar, ya que Time.timeScale ya est� en 0
        }

        public void OnExit(AssemblyGameManager context)
        {
            // No hacemos nada al salir
        }

        public void Update(AssemblyGameManager context)
        {
            // No actualizamos nada mientras estamos en pausa
        }

        public void HandleInput(AssemblyGameManager context)
        {
            // Podemos manejar inputs aqu� si es necesario, pero por ahora lo dejamos vac�o
        }
    }
}