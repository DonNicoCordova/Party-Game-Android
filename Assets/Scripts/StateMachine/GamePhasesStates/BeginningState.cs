
using UnityEngine;
using UnityEngine.AI;

internal class Initialize : IState
{
    public Initialize()
    {
    }
    public void Tick()
    {
    }

    public void OnEnter()
    {
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        Debug.Log("ENTERED BEGINNING");
        GameManager.instance.ShowMessage("¡Bienvenido a tu fiestita!");
        GameManager.instance.CreatePlayers();
    }

    public void OnExit()
    {
        if (GameSystem.instance.initializePhaseDone)
            GameSystem.instance.initializePhaseDone = false;
        GameManager.instance.throwController?.AnimateReadyToPlay();
        Debug.Log("FINISHED BEGINNING");
    }
}