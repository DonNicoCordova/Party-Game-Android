using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Fence TopFence;
    public Fence BotFence;

    public void RemoveBotFence()
    {
        BotFence.Remove();
    }
    public void RemoveTopFence()
    {
        TopFence.Remove();
    }
}
