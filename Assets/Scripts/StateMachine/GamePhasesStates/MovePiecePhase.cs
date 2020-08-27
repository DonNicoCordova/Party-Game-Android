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
        GameManager.instance.ShowMessage("¡Te toca mover!");
    }

    public void OnExit()
    {
    }
}