using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI playerName = null;

    private void Awake()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");
    }
    private void OnDisable() => PlayerPrefs.SetString("PlayerName", playerName.text);
}
