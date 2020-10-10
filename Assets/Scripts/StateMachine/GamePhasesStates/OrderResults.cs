
using UnityEngine;
using Photon.Pun;
internal class OrderResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public OrderResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.orderingResultsPhaseDone = true;
            GameManager.instance?.GetMainPlayer().SetStateDone();

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        //reset state done
        GameManager.instance.ResetStateOnPlayers();
        
        if (GameSystem.instance.orderingResultsPhaseDone)
            GameSystem.instance.orderingResultsPhaseDone = false;
        Debug.Log("ENTERED ORDER RESULT PHASE");
        GameManager.instance.ShowMessage("Así quedaron y que wea");
        GameManager.instance.OrderPlayers();

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.orderingResultsPhaseDone)
            GameSystem.instance.orderingResultsPhaseDone = false;
        UnityEngine.Debug.Log("EXITED ORDER RESULT PHASE");
        GameManager.instance.StartNextRound();
        //ORDENAR LISTA
    }
}