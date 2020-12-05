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

        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.gameOverPhaseTimerDone)
            GameSystem.Instance.gameOverPhaseTimerDone = false;
        Debug.Log("ENTERING GAME OVER PHASE");

        GameManager.Instance.FinishGame();
        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.gameOverPhaseTimerDone)
            GameSystem.Instance.gameOverPhaseTimerDone = false;
        Debug.Log("EXITING FINAL RESULTS");
    }
}