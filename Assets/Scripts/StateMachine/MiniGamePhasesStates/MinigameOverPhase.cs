using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class MinigameOverPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public MinigameOverPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime == 0f)
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player.playerStats.id != 0)
            {
                FallingGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                FallingGameManager.Instance?.photonView.RPC("SetMinigameDone", RpcTarget.MasterClient, player.playerStats.id);
            }
            stayTime = -1f;
        } else if (stayTime > 0f)
        {
            stayTime -= Time.deltaTime;
            stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        }
        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FallingGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }

        FallingGameManager.Instance.ResetStateOnPlayers();
        FallingGameManager.Instance.InitializeGameOver();
        FallingGameManager.Instance.ShowGameResults();
        FallingGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        FallingGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
}