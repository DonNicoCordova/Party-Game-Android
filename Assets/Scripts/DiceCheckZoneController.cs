using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DiceCheckZoneController : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Collider> sideColliders = new List<Collider>();
    private GameManager gameManager;
 
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        sideColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (sideColliders.Contains(other))
        {
            sideColliders.Remove(other);
        }
    }

    private void FixedUpdate()
    {
        if (gameManager != null && !gameManager.throwController.IsThrowFinished())
        {
            if (gameManager.throwController.DicesStopped() && sideColliders.Count == 2)
            {
                Throw actualThrow = new Throw(gameManager.GetMainPlayer().playerStats.id);
                actualThrow.playerNickname = gameManager.GetMainPlayer().photonPlayer.NickName;
                SideStats[] sideStats = new SideStats[2];
                var i = 0;
                foreach(Collider collider in sideColliders)
                {
                    DiceController controller = collider.GetComponentInParent<DiceController>();
                    sideStats[i] = controller.GetSideStats(collider);
                    i++;
                }
                int output = 0;
                foreach (SideStats stat in sideStats)
                {
                    output += 7 - (int)stat.GetValue();
                }
                actualThrow.throwValue = output;
                gameManager.throwController.FinishedThrow();
                string rpcParams = JsonUtility.ToJson(actualThrow);
                gameManager.photonView.RPC("AddThrow", RpcTarget.All, rpcParams);
                gameManager.throwController.actualThrow = actualThrow;
                gameManager.SetMainPlayerMoves(output);
                gameManager.SetThrowText();
                sideColliders.Clear();
            }
        }

    }

}
