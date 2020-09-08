
using UnityEngine;
using UnityEngine.AI;

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
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void OnEnter()
    {
        GameManager.instance.throwText.text = "0";
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        if (GameManager.instance.GetRound() == 0)
        {
            GameManager.instance.ShowMessage("¡Bienvenido a tu fiestita!");
            GameManager.instance.CreatePlayers();
        } else
        {
            GameManager.instance.ShowMessage($"¡Ronda {GameManager.instance.GetRound()}!");
        }
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        GameManager.instance.throwController?.AnimateReadyToPlay();
        GameManager.instance.throwController?.Initialize();
    }
}