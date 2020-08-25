using UnityEngine;
using UnityEngine.AI;

internal class Initialize : IState
{
    public Initialize()
    {
    }
    public void Tick()
    {
        GameManager.instance.throwController?.Initialize();
    }

    public void OnEnter()
    {
        Debug.Log("ENTERING BEGINNING");
    }

    public void OnExit()
    {
        GameManager.instance.throwController?.AnimateReadyToPlay();
        Debug.Log("FINISHED BEGINNING");
    }
}