using UnityEngine;
using UnityEngine.AI;

internal class OrderDecidingPhase : IState
{
    public OrderDecidingPhase()
    {
    }
    public void Tick()
    {
        GameManager.instance.throwController?.CheckInput();
    }

    public void OnEnter()
    {
        if (GameSystem.instance.orderingPhaseDone)
            GameSystem.instance.orderingPhaseDone = false;
        Debug.Log("ENTERING ORDERING");
        GameManager.instance.ShowMessage("Decidamos el orden de juego");
    }

    public void OnExit()
    {
        if (GameSystem.instance.orderingPhaseDone)
            GameSystem.instance.orderingPhaseDone = false;
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED ORDERING");
    }
}