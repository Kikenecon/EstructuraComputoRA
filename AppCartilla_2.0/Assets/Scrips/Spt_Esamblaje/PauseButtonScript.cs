using AssemblyGame;
using UnityEngine;

public class PauseButtonScript : MonoBehaviour
{
    public void OnPauseButtonClicked()
    {
        UIManagerState.Instance.OnPause();
    }
}
