using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class OrderDecidingPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public OrderDecidingPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.initializePhaseTimerDone = true;
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player && !player.playerStats.currentStateFinished)
            {
                GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick()
    {

    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done

        GameManager.Instance.playersLadder.AnimateShuffle();
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.orderingPhaseTimerDone)
            GameSystem.Instance.orderingPhaseTimerDone = false;
        GameManager.Instance.ShowMessage("Decidamos el orden de juego");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.OrderPlayers();
        }
    }

    public void OnExit()
    {
        GameManager.Instance.playersLadder.StopShuffle();
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.orderingPhaseTimerDone)
            GameSystem.Instance.orderingPhaseTimerDone = false;
    }
}