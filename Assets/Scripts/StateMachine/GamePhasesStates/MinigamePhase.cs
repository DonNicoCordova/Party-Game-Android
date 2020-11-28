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
            GameSystem.instance.minigamePhaseTimerDone = true;
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
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
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "PickFallingItemMiniGame");
        }
        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.minigamePhaseTimerDone)
            GameSystem.instance.minigamePhaseTimerDone = false;
        Debug.Log("ENTERING MINIGAME");
        //GameManager.instance.ShowMessage("Ahora deberia estar un juego");

    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.minigamePhaseTimerDone)
            GameSystem.instance.minigamePhaseTimerDone = false;
        Debug.Log("EXITING MINIGAME");
        
    }
}