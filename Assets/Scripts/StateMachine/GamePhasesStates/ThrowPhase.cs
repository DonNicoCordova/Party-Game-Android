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
        GameManager.Instance.throwController?.CheckInput();

        if (stayTime <= 0f)
        {
            GameSystem.Instance.throwPhaseTimerDone = true;

        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }
    public void FixedTick()
    {
        GameManager.Instance.throwController?.CheckIfDicesStopped();
        if (GameManager.Instance.throwController.DicesStopped())
        {
            PlayerController player = GameManager.Instance?.GetMainPlayer();
            if (player)
            {
                GameManager.Instance?.photonView.RPC("SetStateDone", RpcTarget.MasterClient, player.playerStats.id);
            }
        }
    }
    public void OnEnter()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.photonView.RPC("SetCurrentState", RpcTarget.OthersBuffered, this.GetType().Name);
            GameManager.Instance.ClearThrows();
        }
        //reset state done

        GameManager.Instance.ResetStateOnPlayers();
        if (GameSystem.Instance.throwPhaseTimerDone)
            GameSystem.Instance.throwPhaseTimerDone = false;
        Debug.Log("ENTERED THROWPHASE");
        GameManager.Instance.ShowMessage("¡Lanza tus dados!");

        GameManager.Instance.timerBar.SetMaxTime(defaultStayTime);
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        GameManager.Instance.timerBar.SetTimeLeft(stayTime);
        if (GameSystem.Instance.throwPhaseTimerDone)
            GameSystem.Instance.throwPhaseTimerDone = false;
        GameManager.Instance.throwController?.EnableDicesAnimations();
        GameManager.Instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}