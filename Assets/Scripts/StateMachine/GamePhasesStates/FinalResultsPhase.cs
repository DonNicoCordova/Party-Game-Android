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
            GameSystem.instance.finalResultsPhaseTimerDone = true;
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
        if (GameSystem.instance.finalResultsPhaseTimerDone)
            GameSystem.instance.finalResultsPhaseTimerDone = false;
        Debug.Log("ENTERING FINAL RESULTS");
        GameManager.instance.ShowMessage("Final de la ronda! WOOOOOOO");

        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.finalResultsPhaseTimerDone)
            GameSystem.instance.finalResultsPhaseTimerDone = false;
        Debug.Log("EXITING FINAL RESULTS");
    }
}