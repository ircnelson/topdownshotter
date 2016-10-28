using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    protected bool Paused { get; private set; }

    protected virtual void OnPauseGame()
    {
        Paused = true;
    }

    protected virtual void OnResumeGame()
    {
        Paused = false;
    }
}
