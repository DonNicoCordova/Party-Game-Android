using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class FlappyRoyaleGameManager : GenericPunSingletonClass<FlappyRoyaleGameManager>
{
    public GameObject instructionsCanvas;
    public GameObject gameResultsCanvas;
    public GameObject guiCanvas;
    public GameObject explosion;
    public float gravityFactor = 5;
    [Header("Player")]
    public GameObject spaceShipPrefab;
    public Transform spawnPoint;

    private int numberOfPlayers = 0;
    // Start is called before the first frame update
    void Start()
    {
        //LevelLoader.Instance.FadeIn();
        //StartCoroutine(InitializeGuiProcess());
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Physics.gravity = new Vector3(0, -gravityFactor, 0);
    }
    private void Update()
    {
    }
    public bool AllPlayersStateDone()
    {
        if (AllPlayersJoined())
        {
            foreach (PlayerController player in GameManager.Instance.players)
            {
                if (!player.playerStats.currentMinigameStateFinished)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool AllPlayersJoined() => numberOfPlayers == PhotonNetwork.PlayerList.Length;
    [PunRPC]
    private void ImInGame()
    {
        numberOfPlayers++;
        if (AllPlayersJoined())
        {
            SpawnPlayer();
        }
    }
    public void SpawnPlayer()
    {
        //LocationController playerSpawn = playerConfigs[PhotonNetwork.LocalPlayer.ActorNumber - 1].startingLocation;
        //Transform waypoint = playerSpawn.waypoint;
        //GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab, waypoint.position, Quaternion.identity);
        //PlayerController characterScript = playerObj.GetComponent<PlayerController>();
        //characterScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        //characterScript.playerStats.lastSpawnPosition = waypoint.transform;
        //playerSpawn.photonView.RPC("SetOwner", RpcTarget.AllBuffered, characterScript.playerStats.id);
        ////Initialize player
    }
    [PunRPC]
    public void SetCurrentState(string state)
    {
        MinigameSystem.Instance.SetState(state);
    }
    [PunRPC]
    public void SetStateDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentMinigameStateFinished)
            {
                player.playerStats.currentMinigameStateFinished = true;
            }
        }
    }
    [PunRPC]
    public void SetMinigameDone(int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerController player = GameManager.Instance.GetPlayer(playerId);
            if (!player.playerStats.currentMinigameOver)
            {
                player.playerStats.currentMinigameOver = true;
            }
        }
    }
    public void InitializeGameOver()
    {
        MinigameOverController gameOverController = gameResultsCanvas.GetComponent<MinigameOverController>();
        gameOverController.Initialize();
    }
    public void ResetStateOnPlayers()
    {
        foreach (PlayerController player in GameManager.Instance.players)
        {
            player.playerStats.currentMinigameStateFinished = false;
        }
    }
    public void ShowInstructions()
    {
        guiCanvas.SetActive(false);
        instructionsCanvas.SetActive(true);
    }
    public void HideInstructions()
    {

        guiCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);
    }
    public void ShowGameResults()
    {
        guiCanvas.SetActive(false);
        gameResultsCanvas.SetActive(true);
    }
    public System.Collections.IEnumerator InitializeGuiProcess()
    {
        while (!GameManager.Instance.AllPlayersJoined())
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}