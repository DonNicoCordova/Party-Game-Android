using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

internal class ThrowPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public ThrowPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        GameManager.instance.throwController?.CheckInput();

        if (stayTime <= 0f)
        {
            GameSystem.instance.throwPhaseTimerDone = true;

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick()
    {
        GameManager.instance.throwController?.CheckIfDicesStopped();
        if (GameManager.instance.throwController.DicesStopped())
        {
            PlayerController player = GameManager.instance?.GetMainPlayer();
            if (player)
            {
                GameManager.instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            GameManager.instance.ClearThrows();
        }
        //reset state done

        GameManager.instance.ResetStateOnPlayers();
        if (GameSystem.instance.throwPhaseTimerDone)
            GameSystem.instance.throwPhaseTimerDone = false;
        Debug.Log("ENTERED THROWPHASE");
        GameManager.instance.ShowMessage("¡Lanza tus dados!");

        GameManager.instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.instance.throwPhaseTimerDone)
            GameSystem.instance.throwPhaseTimerDone = false;
        GameManager.instance.throwController?.EnableDicesAnimations();
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}