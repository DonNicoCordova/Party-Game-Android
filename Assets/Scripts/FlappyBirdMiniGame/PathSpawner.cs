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
    public GameObject testObstacle;
    private float obstacleDelayTime;
    private static PathSpawner instance;
    public bool enabledToSpawn = false;
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
                Debug.Log("OBSTACLE DELAY TIME <= 0");
                int randomIndex = Random.Range(0, obstacles.Count);
                string obstacle = obstacles[randomIndex];
                int yPosition = Random.Range(-5, 5);
                Debug.Log("CALLING PLACE OBSTACLE RPC");
                this.photonView.RPC("PlaceObstacle", RpcTarget.MasterClient, obstacle, yPosition);

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
    public void PlaceObstacle(string obstacle, int yPosition)
    {
        Debug.Log("CALLING PLACE OBSTACLE");
        Vector3 newSpawnPosition = spawnPosition.position + new Vector3(0, yPosition);
        GameObject obstacleGo = PhotonNetwork.Instantiate(obstacle, newSpawnPosition, Quaternion.identity);
        StartCoroutine(destroyObstacle(obstacleGo));
    }

    private void TestPlaceObstacle(GameObject obstacle, int yPosition)
    {
        Debug.Log("CALLING PLACE OBSTACLE");
        Vector3 newSpawnPosition = spawnPosition.position + new Vector3(0, yPosition);
        GameObject obstacleGo = GameObject.Instantiate(obstacle, newSpawnPosition, Quaternion.identity, transform);
        StartCoroutine(destroyObstacle(obstacleGo));
    }

    public IEnumerator destroyObstacle(GameObject obstacle)
    {
        yield return new WaitForSeconds(5f);
        Destroy(obstacle);
    }
}
