namespace AssemblyGame
{
    public interface IGameState
    {
        void OnEnter(AssemblyGameManager context);
        void OnExit(AssemblyGameManager context);
        void Update(AssemblyGameManager context);
        void HandleInput(AssemblyGameManager context);
    }
}