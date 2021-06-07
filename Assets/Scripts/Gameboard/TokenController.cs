using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenController : MonoBehaviour
{
    public enum TokenPosition
    {
        None, Top, MiddleLeft, MiddleRight, BotRight, BotLeft
    }
    public TokenPosition tokenPosition;
    public Image icon;
    public TextMeshProUGUI textName;
    private Animator _animator;
    private Token _tokenInfo;
    private Image tokenBGImage;
    private bool usable = false;
    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        tokenBGImage = gameObject.GetComponent<Image>();
    }
    public void InitFromToken(Token newToken)
    {
        Hide();
        _tokenInfo = newToken;
        icon.sprite = _tokenInfo.icon;
        textName.text = _tokenInfo.verboseName;
        usable = true;
        _animator.enabled = true;
    }
    public void Disable()
    {
        _animator.enabled = false;
        Hide();
        usable = false;
    }
    public void Hide()
    {
        HideIcon();
        HideText();
        tokenBGImage.enabled = false;
    }
    public void HideIcon()
    {
        icon.enabled = false;
    }

    public void HideText()
    {
        textName.enabled = false;
    }
    public void ShowIcon()
    {
        icon.enabled = true;
    }

    public void ShowText()
    {
        textName.enabled = true;
    }
    public void AnimateShow()
    {
        if (usable)
        {
            switch (tokenPosition)
            {
                case TokenPosition.Top:
                    _animator.SetTrigger("EnterTop");
                    break;
                case TokenPosition.BotRight:
                    _animator.SetTrigger("EnterBotRight");
                    break;
                case TokenPosition.MiddleRight:
                    _animator.SetTrigger("EnterMiddleRight");
                    break;
                case TokenPosition.BotLeft:
                    _animator.SetTrigger("EnterBotLeft");
                    break;
                case TokenPosition.MiddleLeft:
                    _animator.SetTrigger("EnterMiddleLeft");
                    break;
            }
        }
    }

}
