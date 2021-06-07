using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagEvents : MonoBehaviour
{
    private PrizesUI prizesUI;
    private void Start()
    {
        prizesUI = PrizesUI.Instance;
    }
    public void TriggerTopToken()
    {
        prizesUI.TriggerTopToken();
    }
    public void TriggerMiddleLeftToken()
    {
        prizesUI.TriggerMiddleLeftToken();
    }
    public void TriggerMiddleRightToken()
    {
        prizesUI.TriggerMiddleRightToken();
    }
    public void TriggerBotLeftToken()
    {
        prizesUI.TriggerBotLeftToken();
    }
    public void TriggerBotRightToken()
    {
        prizesUI.TriggerBotRightToken();
    }
}
