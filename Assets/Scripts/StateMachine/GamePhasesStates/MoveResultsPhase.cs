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
            GameSystem.instance.moveResultsPhaseTimerDone = true; 
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
        }
        //reset state done
        GameManager.instance.ResetPlayers();
        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.moveResultsPhaseTimerDone)
            GameSystem.instance.moveResultsPhaseTimerDone = false;
        Debug.Log("ENTERED MOVERESULTSPHASE");
        GameManager.instance.ShowMessage("Este mensaje da risa... creo");
        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        GameManager.instance.DisableJoystick();
        stayTime = defaultStayTime;
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.moveResultsPhaseTimerDone)
            GameSystem.instance.moveResultsPhaseTimerDone = false;
        Debug.Log("EXITED MOVERESULTSPHASE");
    }
}