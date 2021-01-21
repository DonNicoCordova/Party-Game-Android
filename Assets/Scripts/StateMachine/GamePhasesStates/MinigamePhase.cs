using UnityEngine;
using Photon.Pun;

internal class MinigamePhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float stayTime;
    public MinigamePhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.Instance.minigamePhaseTimerDone = true;
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
        //reset state done
        if (PhotonNetwork.IsMasterClient)
        {
            GameboardRPCManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            MiniGameScene minigameScene = GameManager.Instance.miniGamesQueue.Dequeue();
            string minigameSceneName = minigameScene.scene;
            NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, minigameSceneName);
        }
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.minigamePhaseTimerDone)
            GameSystem.Instance.minigamePhaseTimerDone = false;
        //GameManager.Instance.ShowMessage("Ahora deberia estar un juego");

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.Instance.minigamePhaseTimerDone)
            GameSystem.Instance.minigamePhaseTimerDone = false;

        if (PhotonNetwork.IsMasterClient)
        {
            NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameboardScene");
        }

    }
}