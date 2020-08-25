using UnityEngine;
using UnityEngine.AI;

internal class OrderDecidingPhase : IState
{
    private readonly Animator _platformAnimator;
    public OrderDecidingPhase()
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