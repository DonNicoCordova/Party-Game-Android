using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomSelectController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roomName = null;
    private PlayersSelectController playerSelectController;
    private NetworkManager netManager;
    private void Start()
    {
        playerSelectController = gameObject.GetComponentInChildren<PlayersSelectController>();
        netManager = NetworkManager.Instance;
    }
    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable() => PlayerPrefs.SetString("RoomName", roomName.text);
}
