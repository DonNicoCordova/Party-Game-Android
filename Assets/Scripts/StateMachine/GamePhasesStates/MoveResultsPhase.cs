using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class MoveResultsPhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float stayTime;
    public MoveResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.moveResultsPhaseTimerDone = true; 
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player && !player.playerStats.currentStateFinished)
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
        GameManager.Instance.ShowMessage("¡Se viene un minijuego!");
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.moveResultsPhaseTimerDone)
            GameSystem.Instance.moveResultsPhaseTimerDone = false;
        GameManager.Instance.timerBar.SetTimeLeft(0);

    }

    public void OnExit()
    {
        GameManager.Instance.SavePlayers();
        GameManager.Instance.SaveBridges();
        stayTime = defaultStayTime;
        if (GameSystem.Instance.moveResultsPhaseTimerDone)
            GameSystem.Instance.moveResultsPhaseTimerDone = false;
    }
}