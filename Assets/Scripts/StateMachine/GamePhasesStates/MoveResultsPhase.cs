using UnityEngine;
using UnityEngine.AI;

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
            GameSystem.instance.moveResultsPhaseDone = true;
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void FixedTick() { }
    public void OnEnter()
    {
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("ENTERED MOVERESULTSPHASE");
        GameManager.instance.ShowMessage("Este mensaje da risa... creo");
        GameManager.instance.StartNextRound();
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.moveResultsPhaseDone)
            GameSystem.instance.moveResultsPhaseDone = false;
        Debug.Log("EXITED MOVERESULTSPHASE");
    }
}