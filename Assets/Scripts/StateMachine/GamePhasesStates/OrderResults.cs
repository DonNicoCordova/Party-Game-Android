
internal class OrderResultsPhase : IState
{

    public OrderResultsPhase()
    {
    }

    public void Tick()
    {
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
        if (GameSystem.instance.orderingResultsPhaseDone)
            GameSystem.instance.orderingResultsPhaseDone = false;
        UnityEngine.Debug.Log("EXITED ORDER RESULT PHASE");
        //ORDENAR LISTA
    }
}