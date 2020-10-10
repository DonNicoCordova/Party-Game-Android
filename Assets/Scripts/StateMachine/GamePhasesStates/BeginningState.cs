
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
internal class Initialize : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public Initialize(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.initializePhaseDone = true;
            GameManager.instance?.GetMainPlayer().SetStateDone();
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void FixedTick() { }
    public void OnEnter()
    {

        //reset state done

        GameManager.instance.ResetStateOnPlayers();
        Debug.Log($"ENTERING STATE {GameManager.instance}");
        GameManager.instance.throwText.text = "0";
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        if (GameManager.instance.GetRound() == 0)
        {
            Debug.Log("CALLING ON ROUND == 0");
            GameManager.instance.ShowMessage("¡Bienvenido a tu fiestita!");
            //GameManager.instance.CreatePlayers();
        }
        else
        {
            Debug.Log("CALLING ON ROUND != 0");
            GameManager.instance.ShowMessage($"¡Ronda {GameManager.instance.GetRound()}!");
        }
    }

    
    public void OnExit()
    {
        Debug.Log("CALLING EXIT OF BEGINNING STATE");
        stayTime = defaultStayTime;
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        GameManager.instance.throwController?.AnimateReadyToPlay();
        GameManager.instance.throwController?.Initialize();
    }
}