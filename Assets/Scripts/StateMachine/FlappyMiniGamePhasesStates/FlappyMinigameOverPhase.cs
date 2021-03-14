using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class FlappyMinigameOverPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public FlappyMinigameOverPhase(float minimumTime)
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
                FlappyRoyaleGameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
                FlappyRoyaleGameManager.Instance?.photonView.RPC("SetMinigameDone", RpcTarget.MasterClient, player.playerStats.id);
            }
            stayTime = -1f;
        } else if (stayTime > 0f)
        {
            stayTime -= Time.deltaTime;
            stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        }
        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FlappyRoyaleGameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }

        FlappyRoyaleGameManager.Instance.ResetStateOnPlayers();
        FlappyRoyaleGameManager.Instance.InitializeGameOver();
        FlappyRoyaleGameManager.Instance.ShowGameResults();
        FlappyRoyaleGameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        FlappyRoyaleGameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
}