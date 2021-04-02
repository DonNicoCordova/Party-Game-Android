using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class PathSpawner : MonoBehaviourPunCallbacks
{
    public List<GameObject> paths = new List<GameObject>();
    public List<string> obstacles = new List<string>();
    public Transform spawnPosition;
    public float defaultDelayTime = 1f;
    private float obstacleDelayTime;
    private static PathSpawner instance;
    private bool enabledToSpawn = false;
    public static PathSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PathSpawner>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PathSpawner).Name;
                    instance = obj.AddComponent<PathSpawner>();
                }
            }
            return instance;
        }
    }
    private void Start()
    {
        obstacleDelayTime = 8 * defaultDelayTime;
        if (paths != null && paths.Count > 0)
        {
            paths = paths.OrderBy(p => p.transform.position.x).ToList();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Path"))
        {
            GameObject firstPath = paths.First();
            paths.Remove(firstPath);
            MovePath(firstPath);
            paths.Add(firstPath);
        }
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (obstacleDelayTime <= 0f && enabledToSpawn)
            {
                int randomIndex = Random.Range(0, obstacles.Count);
                string obstacle = obstacles[randomIndex];
                this.photonView.RPC("PlaceObstacle", RpcTarget.MasterClient, obstacle);
                obstacleDelayTime = 4 * defaultDelayTime;
            }
            obstacleDelayTime -= Time.deltaTime;
            obstacleDelayTime = Mathf.Clamp(obstacleDelayTime, 0f, Mathf.Infinity);
        }
    }
    public void MovePath(GameObject path)
    {
        path.transform.position = spawnPosition.position;
    }
    public void EnableSpawning()
    {
        enabledToSpawn = true;
    }
    [PunRPC]
    public void PlaceObstacle(string obstacle)
    {
        GameObject obstacleGo = PhotonNetwork.Instantiate(obstacle, spawnPosition.position, Quaternion.identity);
        StartCoroutine(destroyObstacle(obstacleGo));
    }

    public IEnumerator destroyObstacle(GameObject obstacle)
    {
        yield return new WaitForSeconds(5f);
        Destroy(obstacle);
    }
}
