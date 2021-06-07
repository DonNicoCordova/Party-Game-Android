using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PrizesUI : GenericDestroyableSingletonClass<PrizesUI>
{
    public TokenController[] displayTokens;
    public Animator bagAnimator;
    public Image bagImage;
    private TokenController topToken;
    private TokenController middleRightToken;
    private TokenController middleLeftToken;
    private TokenController botRightToken;
    private TokenController botLeftToken;

    public void Start()
    {
        topToken = displayTokens.First(element => element.tokenPosition == TokenController.TokenPosition.Top);
        middleRightToken = displayTokens.First(element => element.tokenPosition == TokenController.TokenPosition.MiddleRight);
        middleLeftToken = displayTokens.First(element => element.tokenPosition == TokenController.TokenPosition.MiddleLeft);
        botRightToken = displayTokens.First(element => element.tokenPosition == TokenController.TokenPosition.BotRight);
        botLeftToken = displayTokens.First(element => element.tokenPosition == TokenController.TokenPosition.BotLeft);
    }
    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            GivePrizesToMainPlayer(5);
        } else if (Input.GetKeyDown("h"))
        {
            Hide();
        } else if (Input.GetKeyDown("s"))
        {
            Show();
        }
    }
    public void Hide()
    {
        bagAnimator.enabled = false;
        bagImage.enabled = false;
        foreach (TokenController token in displayTokens)
        {
            token.Disable();
        }
    }
    public void Show()
    {
        bagAnimator.enabled = true;
    }
    public void InitFromPrizes(Token[] tokens)
    {
        switch (tokens.Length)
        {
            case 1:
                topToken.InitFromToken(tokens[0]);
                middleLeftToken.Disable();
                middleRightToken.Disable();
                botLeftToken.Disable();
                botRightToken.Disable();
                break;
            case 3:

                topToken.InitFromToken(tokens[0]);
                middleLeftToken.InitFromToken(tokens[1]);
                middleRightToken.InitFromToken(tokens[2]);
                botLeftToken.Disable();
                botRightToken.Disable();
                break;
            case 5:
                TokenController tokenController;
                for (var i = 0; i < 5; i++)
                {
                    tokenController = displayTokens[i];
                    tokenController.InitFromToken(tokens[i]);
                }
                break;
        }
    }
    public void TriggerTopToken()
    {
        topToken.AnimateShow();
    }
    public void TriggerMiddleLeftToken()
    {
        middleLeftToken.AnimateShow();
    }
    public void TriggerMiddleRightToken()
    {
        middleRightToken.AnimateShow();
    }
    public void TriggerBotLeftToken()
    {
        botLeftToken.AnimateShow();
    }
    public void TriggerBotRightToken()
    {
        botRightToken.AnimateShow();
    }
    public void GivePrizesToMainPlayer(int numberOfTokens)
    {
        Debug.Log($"TRYING TO GIVE PRIZES TO MAIN PLAYER: {displayTokens}");
        foreach (TokenController tokenController in displayTokens)
        {
            tokenController.Disable();
        }
        PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
        Token[] newTokens = mainPlayer.inventory.AddRandomTokens(numberOfTokens);
        InitFromPrizes(newTokens);
        bagAnimator.enabled = true;
        bagAnimator.SetTrigger("Open");
    }

}
