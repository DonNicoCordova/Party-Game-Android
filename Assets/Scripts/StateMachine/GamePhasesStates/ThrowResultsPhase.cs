using UnityEngine;
using UnityEngine.AI;

internal class ThrowResultsPhase : IState
{
    private readonly GameManager _gameManager;

    public ThrowResultsPhase(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Tick()
    {
        GameManager.instance.ShowDicesOnCamera();
    }

    public void OnEnter()
    {
        Debug.Log("ENTERED THROWRESULTSPHASE");
    }

    public void OnExit()
    {
        Debug.Log("FINISHED THROWRESULTSPHASE");
    }
}