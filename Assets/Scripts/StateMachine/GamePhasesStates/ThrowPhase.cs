using UnityEngine;
using UnityEngine.AI;

internal class ThrowPhase : IState
{
    private readonly Animator _platformAnimator;
    public ThrowPhase()
    {
    }
    public void Tick()
    {
        GameManager.instance.throwController?.CheckInput();
    }

    public void OnEnter()
    {
        if (GameSystem.instance.throwPhaseDone)
            GameSystem.instance.throwPhaseDone = false;
        Debug.Log("ENTERED THROWPHASE");
        GameManager.instance.throwController?.DisableDicesAnimations();
        GameManager.instance.ShowMessage("¡Lanza tus dados!");
        GameManager.instance.StartNextRound();
    }

    public void OnExit()
    {
        if (GameSystem.instance.throwPhaseDone)
            GameSystem.instance.throwPhaseDone = false;
        GameManager.instance.throwController?.EnableDicesAnimations();
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}