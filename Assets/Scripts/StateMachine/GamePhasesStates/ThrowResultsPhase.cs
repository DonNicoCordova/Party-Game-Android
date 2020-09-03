
using UnityEngine;

internal class ThrowResultsPhase : IState
{
    public ThrowResultsPhase()
    {
    }

    public void Tick()
    {
    }

    public void OnEnter()
    {
        if (GameSystem.instance.throwResultsPhaseDone)
            GameSystem.instance.throwResultsPhaseDone = false;
        Debug.Log("ENTERED THROWRESULT");
        GameManager.instance.ShowMessage("¡Que mala cuea! ajkajskkadj");
    }

    public void OnExit()
    {
        if (GameSystem.instance.throwResultsPhaseDone)
            GameSystem.instance.throwResultsPhaseDone = false;
        Debug.Log("EXITED THROWRESULT");
    }
}