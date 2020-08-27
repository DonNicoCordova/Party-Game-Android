
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
        GameManager.instance.ShowMessage("¡Bienvenido a tu fiestita!");
        GameManager.instance.CreatePlayers();
    }

    public void OnExit()
    {
        GameManager.instance.throwController?.AnimateReadyToPlay();
        Debug.Log("FINISHED BEGINNING");
    }
}