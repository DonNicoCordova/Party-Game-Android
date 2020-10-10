using UnityEngine;
using Photon.Pun;

internal class ThrowResultsPhase : IState
{
    private float defaultStayTime = 5f;
    private float stayTime;
    public ThrowResultsPhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.throwResultsPhaseDone = true;
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
        if (GameSystem.instance.throwResultsPhaseDone)
            GameSystem.instance.throwResultsPhaseDone = false;
        Debug.Log("ENTERED THROWRESULT");
        GameManager.instance.ShowMessage("¡Que mala cuea! ajkajskkadj");
    }
    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.throwResultsPhaseDone)
            GameSystem.instance.throwResultsPhaseDone = false;
        Debug.Log("EXITED THROWRESULT");
    }
}