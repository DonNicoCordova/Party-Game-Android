using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject inGameMenu;
    
    public void onSettingsButtonClick()
    {
        if (inGameMenu.activeSelf)
        {
            inGameMenu.SetActive(false);
        } else
        {
            inGameMenu.SetActive(true);
        }
    }
}
