using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class Inventory: MonoBehaviour, IPunObservable
{
    public List<TokenInInventory> tokens;
    public int inventorySize;
    public event EventHandler<TokensChangedArgs> TokensChanged;
    public void Start()
    {
        tokens = new List<TokenInInventory>();
    }
    public void AddToken(Token tokenToAdd)
    {
        bool tokenExists = tokens.Exists(token => token.tokenCode == tokenToAdd.code);
        if (tokenExists)
        {
            TokenInInventory tokenInInventory = tokens.First(token => token.tokenCode == tokenToAdd.code);
            tokenInInventory.amount += 1;
        } else
        {
            TokenInInventory newTokenInInventory = new TokenInInventory() { amount = 1, tokenCode = tokenToAdd.code };
            tokens.Add(newTokenInInventory);
        }
        if (TokensChanged != null)
            TokensChanged(this, new TokensChangedArgs(tokenToAdd.code));
    }
    public void SpendToken(Token tokenToSpend)
    {
        bool tokenExists = tokens.Exists(token => token.tokenCode == tokenToSpend.code);
        if (tokenExists)
        {
            TokenInInventory tokenInInventory = tokens.First(token => token.tokenCode == tokenToSpend.code);
            if (tokenInInventory.amount > 0)
            {
                tokenInInventory.amount -= 1;
            }
        }
        if (TokensChanged != null)
            TokensChanged(this, new TokensChangedArgs(tokenToSpend.code));
    }
    public Token[] AddRandomTokens(int amount)
    {
        var tokenCount = 0;
        Token[] addedTokens = new Token[amount];
        while (tokenCount < amount)
        {
            float chance = UnityEngine.Random.value;
            Token tokenToAdd = null;
            //Chance for Cut 20%
            if (chance < 0.20)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.Cut);
            }
            //Chance for Build 16%
            else if (chance < 0.36)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.Spawn);
            }
            //Chance for AddEnergy 16%
            else if (chance < 0.52)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.AddEnergy);
            }
            //Chance for StealEnergy 16%
            else if (chance < 0.68)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.StealEnergy);
            }
            //Chance for PaintBomb 16%
            else if (chance < 0.84)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.Paint);
            }
            //Chance for Teleport 16%
            else if (chance < 1)
            {
                tokenToAdd = SkillsUI.Instance.GetToken(Skill.Teleport);
            }
            
            if (tokenToAdd != null)
            {
                addedTokens[tokenCount] = tokenToAdd;
                AddToken(tokenToAdd);
                tokenCount += 1;
            }
        }
        return addedTokens;
    }
    public int GetTokenAmount(string tokenCode)
    {
        bool tokenExists = tokens.Any(token => token.tokenCode == tokenCode);
        if (tokenExists)
        {
            TokenInInventory token = tokens.First(token => token.tokenCode == tokenCode);
            return token.amount;
        } else
        {
            return 0;
        }
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
            List<TokenInInventory> receivedTokens = JsonUtility.FromJson<TokenInInventory[]>(tokensObj).ToList();
            tokens = receivedTokens;
        }

    }
    public class TokensChangedArgs : EventArgs
    {
        public TokensChangedArgs(string updatedTokenCode)
        {
            TokenCode = updatedTokenCode;
        }
        public string TokenCode { get; private set; }
    }
}

[System.Serializable]
public class TokenInInventory
{
    [SerializeField]
    public int amount;
    [SerializeField]
    public string tokenCode;

}

[System.Serializable]
public class Token
{
    [SerializeField]
    public string code;
    [SerializeField]
    public string verboseName;
    [SerializeField]
    public Skill skillToUse;
    public Sprite icon;
    public Sprite iconWithBackground;
}