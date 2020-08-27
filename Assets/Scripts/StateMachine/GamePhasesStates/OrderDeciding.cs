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
        Debug.Log("ENTERED ORDERING PHASE");
        GameManager.instance.ShowMessage("Decidamos el orden de juego");
    }

    public void OnExit()
    {
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED ORDERING");
    }
}