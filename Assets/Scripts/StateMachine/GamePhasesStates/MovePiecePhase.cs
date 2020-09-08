using UnityEngine;
using UnityEngine.AI;

internal class MovePiecePhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float stayTime;
    public MovePiecePhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.movePiecePhaseDone = true;
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void OnEnter()
    {
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        Debug.Log("ENTERED MOVEPIECE");
        GameManager.instance.ShowMessage("¡Te toca mover!");
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.movePiecePhaseDone)
            GameSystem.instance.movePiecePhaseDone = false;
        Debug.Log("EXITED MOVEPIECE");
    }
}