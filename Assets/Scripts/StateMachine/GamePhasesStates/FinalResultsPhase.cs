using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FinalResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public FinalResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.finalResultsPhaseTimerDone = true;
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameboardRPCManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
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
        if (GameSystem.Instance.finalResultsPhaseTimerDone)
            GameSystem.Instance.finalResultsPhaseTimerDone = false;
        GameManager.Instance.ShowMessage("Final de la ronda! WOOOOOOO");
        GameManager.Instance.timerBar.SetTimeLeft(0);


    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        if (GameSystem.Instance.finalResultsPhaseTimerDone)
            GameSystem.Instance.finalResultsPhaseTimerDone = false;
    }
}