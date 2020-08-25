using UnityEngine;
using UnityEngine.AI;

internal class OrderReultsPhase : IState
{

    public OrderReultsPhase()
    {
    }

    public void Tick()
    {
        if (!GameManager.instance.DiceOnDisplay())
            GameManager.instance.ShowDicesOnCamera();
    }

    public void OnEnter()
    {
        Debug.Log("ENTERED THROWRESULTSPHASE");
    }

    public void OnExit()
    {
        Debug.Log("FINISHED THROWRESULTSPHASE");
    }
}