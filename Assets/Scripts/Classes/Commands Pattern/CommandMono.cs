using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandMono: MonoBehaviour
{
    public abstract void Execute();
    public abstract void Reset();
    public abstract bool IsFinished { get; }
    public abstract bool Failed { get; }
}
