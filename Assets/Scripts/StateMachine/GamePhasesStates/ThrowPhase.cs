using UnityEngine;
using UnityEngine.AI;

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
            GameSystem.instance.throwPhaseDone = true;
        }
        stayTime -= Time.deltaTime;
        stayTime = Mathf.Clamp(stayTime, 0f, Mathf.Infinity);
    }
    public void FixedTick()
    {
        GameManager.instance.throwController?.CheckIfDicesStopped();
    }
    public void OnEnter()
    {
        if (GameSystem.instance.throwPhaseDone)
            GameSystem.instance.throwPhaseDone = false;
        Debug.Log("ENTERED THROWPHASE");
        GameManager.instance.ShowMessage("¡Lanza tus dados!");
    }

    public void OnExit()
    {
        stayTime = defaultStayTime;
        if (GameSystem.instance.throwPhaseDone)
            GameSystem.instance.throwPhaseDone = false;
        GameManager.instance.throwController?.EnableDicesAnimations();
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}