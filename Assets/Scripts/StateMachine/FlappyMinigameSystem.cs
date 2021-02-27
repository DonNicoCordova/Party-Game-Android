using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class FlappyMinigameSystem : MonoBehaviour
{
    public static FlappyMinigameSystem Instance;
    private StateMachine _stateMachine;
    private GameManager _gameManager;
    private List<IState> phases = new List<IState>();

    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("More than one GameSystem in scene!");
            return;
        }
        Instance = this;
        _gameManager = GameManager.Instance;
        _stateMachine = new StateMachine();

        var instructionsPhase = new FlappyInstructionsPhase(10f);
        phases.Add(instructionsPhase);
        var playPhase = new FlappyPlayPhase();
        phases.Add(playPhase);
        var gameOverPhase = new FlappyMinigameOverPhase(10f);
        phases.Add(gameOverPhase);

        At(instructionsPhase, playPhase, AllPlayersReady());
        At(playPhase, gameOverPhase, DonePlaying());
        
        StartCoroutine(Setup(instructionsPhase));
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
    }
    Func<bool> AllPlayersReady() => () =>
    {
        //Change to make sure the minigame has ended, for now checks the same as gameboard phase done
        if (PhotonNetwork.IsMasterClient)
        {
            return GameManager.Instance.AllPlayersMinigameStateDone();
        }
        else
        {
            return false;
        }
    };

    Func<bool> DonePlaying() => () =>
    {
        //Change to make sure the minigame has ended, for now checks the same as gameboard phase done
        if (PhotonNetwork.IsMasterClient)
        {

            return FlappyRoyaleGameManager.Instance.LastPlayerDied();
        }
        else
        {
            return false;
        }
    };

    private void Update() => _stateMachine.Tick();
    private void FixedUpdate() => _stateMachine.FixedTick();
    private IEnumerator Setup(IState initialState)
    {

        yield return new WaitForSeconds(0.5f);
        while (!FlappyRoyaleGameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
        _stateMachine.SetState(initialState);
    }

    public void SetState(string state)
    {
        foreach (IState phase in phases)
        {
            if (phase.GetType().Name == state)
            {
                _stateMachine.SetState(phase);
                break;
            }
        }
    }
    public string GetCurrentStateName()
    {
        if (_stateMachine.GetCurrentState() != null)
        {
            return _stateMachine?.GetCurrentState().GetType().Name;
        } else
        {
            return "None";
        }
    }
}