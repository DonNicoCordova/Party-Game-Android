using UnityEngine;
using UnityEngine.AI;

internal class FinalResultsPhase : IState
{
    public void Tick()
    {

    }

    public void OnEnter()
    {

        GameManager.instance.ShowMessage("Final de la ronda! WOOOOOOO");
    }

    public void OnExit()
    {
    }
}