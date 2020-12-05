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
        //reset state done
        GameManager.Instance.ResetPlayers();
        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.moveResultsPhaseTimerDone)
            GameSystem.Instance.moveResultsPhaseTimerDone = false;
        Debug.Log("ENTERED MOVERESULTSPHASE");
        GameManager.Instance.ShowMessage("Este mensaje da risa... creo");
        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        GameManager.Instance.DisableJoystick();
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.moveResultsPhaseTimerDone)
            GameSystem.Instance.moveResultsPhaseTimerDone = false;
        Debug.Log("EXITED MOVERESULTSPHASE");
    }
}