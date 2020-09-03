using UnityEngine;
using UnityEngine.AI;

internal class MovePiecePhase : IState
{
    private readonly GameObject _gameManager;

    public void Tick()
    {
    }

    public void OnEnter()
    {
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        Debug.Log("ENTERED MOVEPIECE");
        GameManager.instance.ShowMessage("¡Te toca mover!");
    }

    public void OnExit()
    {
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        Debug.Log("EXITED MOVEPIECE");
    }
}