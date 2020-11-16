
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
            GameSystem.instance.orderingResultsPhaseTimerDone = true;
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        GameManager.instance.ResetStateOnPlayers();
        
        if (GameSystem.instance.orderingResultsPhaseTimerDone)
            GameSystem.instance.orderingResultsPhaseTimerDone = false;
        Debug.Log("ENTERED ORDER RESULT PHASE");
        GameManager.instance.ShowMessage("Así quedaron y que wea");
        GameManager.instance.OrderPlayers();

        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.orderingResultsPhaseTimerDone)
            GameSystem.instance.orderingResultsPhaseTimerDone = false;
        UnityEngine.Debug.Log("EXITED ORDER RESULT PHASE");
        //ORDENAR LISTA
    }
}