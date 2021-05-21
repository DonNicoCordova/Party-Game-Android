using UnityEngine;
using System;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Inventory: MonoBehaviour, IPunObservable
{
    public string[] tokens;
    public int inventorySize;

    public void Start()
    {
        tokens = new string[inventorySize];
    }
    
    public void SaveState()
    {

    }
    public void LoadState()
    {

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            string tokensObj = JsonUtility.ToJson(tokens);
            stream.SendNext(tokensObj);
        }
        else if (stream.IsReading)
        {
            string tokensObj = (string)stream.ReceiveNext();
            string[] receivedTokens = JsonUtility.FromJson<string[]>(tokensObj);
            tokens = receivedTokens;
        }

    }
}

[System.Serializable]
public class Token
{
    [SerializeField]
    public string code;
    [SerializeField]
    public Skill skillToUse;
    public Image icon;
}