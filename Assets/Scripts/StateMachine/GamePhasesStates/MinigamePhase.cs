using UnityEngine;
using UnityEngine.AI;

internal class MinigamePhase : IState
{
    private readonly GameObject _gameManager;

    public void Tick()
    {
    }

    public void OnEnter()
    {
        GameManager.instance.ShowMessage("Ahora deberia estar un juego");
    }

    public void OnExit()
    {
    }
}