using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject inGameMenu;
    // Start is called before the first frame update
    
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
