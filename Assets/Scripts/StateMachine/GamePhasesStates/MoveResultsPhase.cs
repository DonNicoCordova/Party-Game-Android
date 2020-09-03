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
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("ENTERED MOVERESULTSPHASE");
        GameManager.instance.ShowMessage("Este mensaje da risa... creo");
    }

    public void OnExit()
    {
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("EXITED MOVERESULTSPHASE");
    }
}