
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
            GameSystem.Instance.orderingResultsPhaseTimerDone = true;
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        GameManager.Instance.ResetStateOnPlayers();
        
        if (GameSystem.Instance.orderingResultsPhaseTimerDone)
            GameSystem.Instance.orderingResultsPhaseTimerDone = false;
        GameManager.Instance.OrderPlayers();
        GameManager.Instance.ShowMessage("Ya estan todos ordenaditos");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.orderingResultsPhaseTimerDone)
            GameSystem.Instance.orderingResultsPhaseTimerDone = false;
        //ORDENAR LISTA
    }
}