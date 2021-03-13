using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class GameOverPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public GameOverPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.gameOverPhaseTimerDone = true;
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

        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.gameOverPhaseTimerDone)
            GameSystem.Instance.gameOverPhaseTimerDone = false;

        GameManager.Instance.FinishGame();
        GameManager.Instance.timerBar.SetTimeLeft(0);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        if (GameSystem.Instance.gameOverPhaseTimerDone)
            GameSystem.Instance.gameOverPhaseTimerDone = false;
    }
}