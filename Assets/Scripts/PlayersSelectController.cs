using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayersSelectController : MonoBehaviour
{
    [Range(1, 8)] public int defaultPlayers = 2;
    public TextMeshProUGUI playersText;

    private int actualPlayers;
    private void Start()
    {
        actualPlayers = defaultPlayers;
    }
    // Update is called once per frame
    private void Update()
    {
        if (playersText.text != actualPlayers.ToString())
        {
            playersText.text = actualPlayers.ToString();
        }
    }

    public void AddPlayer() => actualPlayers = Mathf.Clamp(actualPlayers+1, 1, 8);
    public void SubstractPlayer() => actualPlayers = Mathf.Clamp(actualPlayers-1,1,8);
}
