using UnityEngine;
using UnityEngine.AI;

internal class Begin : IState
{
    private readonly GameManager _gameManager;

    private BoxAccelController throwController;
    public Begin(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Tick()
    {
        GameManager.instance.throwController?.Initialize();
    }

    public void OnEnter()
    {
        Debug.Log("ENTERING BEGINNING");
    }

    public void OnExit()
    {
        Debug.Log("FINISHED BEGINNING");
    }
}