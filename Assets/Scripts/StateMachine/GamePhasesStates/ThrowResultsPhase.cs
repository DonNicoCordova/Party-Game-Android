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
        if (GameSystem.Instance.throwResultsPhaseTimerDone)
            GameSystem.Instance.throwResultsPhaseTimerDone = false;
        int mainPlayerThrow = GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft();
        if (mainPlayerThrow >= 2 && mainPlayerThrow <= 5)
        {
            GameManager.Instance.ShowMessage("¡Que mala cuea!");
        }
        else if (mainPlayerThrow > 5 && mainPlayerThrow <= 8)
        {
            GameManager.Instance.ShowMessage("¡Bastante bien!");
        }
        else if (mainPlayerThrow > 8 && mainPlayerThrow <= 12)
        {
            GameManager.Instance.ShowMessage("¡WOW! Juegate un kino...");
        }
        GameManager.Instance.timerBar.SetTimeLeft(0);
    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.Instance.throwResultsPhaseTimerDone)
            GameSystem.Instance.throwResultsPhaseTimerDone = false;
    }
}