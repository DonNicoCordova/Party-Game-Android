using UnityEngine;
using UnityEngine.AI;

internal class ThrowPhase : IState
{
    private readonly Animator _platformAnimator;
    public ThrowPhase()
    {
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
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}