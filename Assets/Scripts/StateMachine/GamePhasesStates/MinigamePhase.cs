using UnityEngine;
using UnityEngine.AI;

internal class MinigamePhase : IState
{
    private readonly GameObject _gameManager;

    public void Tick()
    {
    }

    public void OnEnter()
    {
        if (GameSystem.instance.minigamePhaseDone)
            GameSystem.instance.minigamePhaseDone = false;
        Debug.Log("ENTERING MINIGAME");
        GameManager.instance.ShowMessage("Ahora deberia estar un juego");
    }

    public void OnExit()
    {
        if (GameSystem.instance.minigamePhaseDone)
            GameSystem.instance.minigamePhaseDone = false;
        Debug.Log("EXITING MINIGAME");
    }
}