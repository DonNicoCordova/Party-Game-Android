using UnityEngine;
using UnityEngine.AI;

internal class FinalResultsPhase : IState
{
    public void Tick()
    {

    }

    public void OnEnter()
    {
        if (GameSystem.instance.finalResultsPhaseDone)
            GameSystem.instance.finalResultsPhaseDone = false;
        Debug.Log("ENTERING FINAL RESULTS");
        GameManager.instance.ShowMessage("Final de la ronda! WOOOOOOO");
    }

    public void OnExit()
    {
        if (GameSystem.instance.finalResultsPhaseDone)
            GameSystem.instance.finalResultsPhaseDone = false;
        Debug.Log("EXITING FINAL RESULTS");
    }
}