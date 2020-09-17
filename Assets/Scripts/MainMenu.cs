using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameObject playerSelectMenu;
    public GameObject mainMenu;
    public void PlayGame()
    {
        levelLoader.LoadNextLevel();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        playerSelectMenu.SetActive(false);
    }
    public void ShowPlayerMenu()
    {

        mainMenu.SetActive(false);
        playerSelectMenu.SetActive(true);
    }
}
