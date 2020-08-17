using UnityEngine;
using UnityEngine.AI;

internal class ThrowPhase : IState
{
    private readonly GameManager _gameManager;
    public ThrowPhase(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Tick()
    {
    }

    public void OnEnter()
    {
        Debug.Log("ENTERED THROWPHASE");
    }

    public void OnExit()
    {

        Debug.Log("FINISHED THROWPHASE");
    }
}