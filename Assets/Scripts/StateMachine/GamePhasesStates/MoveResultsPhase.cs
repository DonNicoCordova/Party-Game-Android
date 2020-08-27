using UnityEngine;
using UnityEngine.AI;

internal class MoveResultsPhase : IState
{
    private readonly GameObject _gameManager;

    public void Tick()
    {
    }

    public void OnEnter()
    {
        GameManager.instance.ShowMessage("Este mensaje da risa... creo");
    }

    public void OnExit()
    {
    }
}