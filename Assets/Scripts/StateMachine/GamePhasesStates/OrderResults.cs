
using UnityEngine;

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
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void OnEnter()
    {
        if (GameSystem.instance.orderingResultsPhaseDone)
            GameSystem.instance.orderingResultsPhaseDone = false;
        UnityEngine.Debug.Log("ENTERED ORDER RESULT PHASE");
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