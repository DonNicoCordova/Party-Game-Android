using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DiceCheckZoneController : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Collider> sideColliders = new List<Collider>();
 
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
        if (GameManager.Instance != null && GameManager.Instance.throwController != null && !GameManager.Instance.throwController.IsThrowFinished())
        {
            if (GameManager.Instance.throwController.DicesStopped() && sideColliders.Count == 2)
            {
                Throw actualThrow = new Throw(GameManager.Instance.GetMainPlayer().playerStats.id);
                actualThrow.playerNickname = GameManager.Instance.GetMainPlayer().photonPlayer.NickName;
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
                GameManager.Instance.throwController.FinishedThrow();
                string rpcParams = JsonUtility.ToJson(actualThrow);
                GameboardRPCManager.Instance.photonView.RPC("AddThrow", RpcTarget.All, rpcParams);
                GameManager.Instance.throwController.actualThrow = actualThrow;
                GameManager.Instance.SetMainPlayerEnergy(output);
                sideColliders.Clear();
            }
        }

    }

}
