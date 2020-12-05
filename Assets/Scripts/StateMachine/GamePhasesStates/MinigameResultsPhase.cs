using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class MinigameResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public MinigameResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }

    public void Tick()
    {
        if (stayTime <= 0f)
        {
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
        GameManager.Instance.ShowMessage("FELICIDADES! te ganaste todas estas cosas.");
        Debug.Log("RESETING STATE ON PLAYERS");
        Debug.Log("Before");
        foreach (PlayerController player in GameManager.Instance.players)
        {
            Debug.Log(player.playerStats.currentStateFinished);
        }
        GameManager.Instance.ResetStateOnPlayers();
        Debug.Log("After");
        foreach (PlayerController player in GameManager.Instance.players)
        {
            Debug.Log(player.playerStats.currentStateFinished);
        }
        Debug.Log("ENTERING MINIGAME RESULTS PHASE");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);

    }
    public void OnExit()
    {
        stayTime = defaultStayTime;

        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        Debug.Log("EXITING MINIGAME RESULTS");
    }
}