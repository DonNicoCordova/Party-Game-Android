using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class Saw : MonoBehaviourPunCallbacks
{
    public Animator animator;
    private Queue<Command> _commands = new Queue<Command>();
    private Queue<Command> _priorityCommands = new Queue<Command>();
    private Command _currentCommand;
    [SerializeField]
    private float defaultRetryTime = 1f;
    private float retryTime;
    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        retryTime = defaultRetryTime;
    }
    private void Update()
    {
        ProcessCommands();
    }
    public void ProcessCommands()
    {
        if (_currentCommand != null && _currentCommand.IsFinished == false)
        {
            _currentCommand.Execute();
            return;
        }
        if (_currentCommand != null && _currentCommand.Failed)
        {
            _currentCommand.Reset();
            _priorityCommands.Enqueue(_currentCommand);
            _currentCommand = null;
            return;
        }
        if (_commands.Any() == false && _priorityCommands.Any() == false)
            return;
        if (_priorityCommands.Count > 0)
        {
            _currentCommand = _priorityCommands.Dequeue();
            _currentCommand.Execute();
        }
        else if (_commands.Count > 0)
        {
            _currentCommand = _commands.Dequeue();
            if (_currentCommand.Failed)
            {   
                _currentCommand.Reset();
                _priorityCommands.Enqueue(_currentCommand);
                _currentCommand = null;
            }
        }
    }
}
