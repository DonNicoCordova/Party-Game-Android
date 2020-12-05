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
                GameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
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
            GameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "PickFallingItemMiniGame");
        }
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.minigamePhaseTimerDone)
            GameSystem.Instance.minigamePhaseTimerDone = false;
        Debug.Log("ENTERING MINIGAME");
        //GameManager.Instance.ShowMessage("Ahora deberia estar un juego");

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.Instance.minigamePhaseTimerDone)
            GameSystem.Instance.minigamePhaseTimerDone = false;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("CHANGING SCENE TO GAMEBOARD SCENE");
            NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameboardScene");
        }

        Debug.Log("EXITING MINIGAME");
        
    }
}