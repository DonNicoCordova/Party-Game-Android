using UnityEngine;
using Photon.Pun;

internal class ThrowResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public ThrowResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.throwResultsPhaseTimerDone = true;
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
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
            GameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.throwResultsPhaseTimerDone)
            GameSystem.Instance.throwResultsPhaseTimerDone = false;
        Debug.Log("ENTERED THROWRESULT");
        GameManager.Instance.ShowMessage("¡Que mala cuea! ajkajskkadj");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.throwResultsPhaseTimerDone)
            GameSystem.Instance.throwResultsPhaseTimerDone = false;
        Debug.Log("EXITED THROWRESULT");
    }
}