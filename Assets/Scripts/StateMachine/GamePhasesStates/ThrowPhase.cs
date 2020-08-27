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
        Debug.Log("ENTERED THROWPHASE");
        GameManager.instance.ShowMessage("¡Lanza tus dados!");
        GameManager.instance.StartNextRound();
    }

    public void OnExit()
    {
        GameManager.instance.throwController?.AnimateFinishedThrow();
        Debug.Log("FINISHED THROWPHASE");
    }
}