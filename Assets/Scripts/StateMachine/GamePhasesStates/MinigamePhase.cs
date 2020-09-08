using UnityEngine;

internal class MinigamePhase : IState
{
    private readonly GameObject _gameManager;
    private float defaultStayTime = 5f;
    private float stayTime;
    public MinigamePhase(float minimumTime)
    {
        defaultStayTime = minimumTime;
        stayTime = defaultStayTime;
    }
    public void Tick()
    {
        if (stayTime <= 0f)
        {
            GameSystem.instance.minigamePhaseDone = true;
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }

    public void OnEnter()
    {
        if (GameSystem.instance.minigamePhaseDone)
            GameSystem.instance.minigamePhaseDone = false;
        Debug.Log("ENTERING MINIGAME");
        GameManager.instance.ShowMessage("Ahora deberia estar un juego");
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.minigamePhaseDone)
            GameSystem.instance.minigamePhaseDone = false;
        Debug.Log("EXITING MINIGAME");
    }
}