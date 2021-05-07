using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
public class PathSpawner : MonoBehaviourPunCallbacks
{
    public List<GameObject> paths = new List<GameObject>();
    public string singleObstacle = "SingleObstacle";
    public string wallObstacle = "WallObstacle";
    public Transform spawnPosition;
    public Transform wallSpawnPoint;
    public GameObject testObstacle;
    public float defaultObstacleDelay = 1f;
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
        if (paths != null && paths.Count > 0)
        {
            paths = paths.OrderBy(p => p.transform.position.x).ToList();
        }
        obstacleDelayTime = defaultObstacleDelay;
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
                int yPosition = Random.Range(-5, 5);
                float chance = Random.value;
                float singleChance = FlappyRoyaleGameManager.Instance.actualDifficulty.singleObstacleChance;
                float wallChance = FlappyRoyaleGameManager.Instance.actualDifficulty.wallObstacleChance;
                float israChance = FlappyRoyaleGameManager.Instance.actualDifficulty.israObstacleChance;
                if (chance < israChance)
                {
                    this.photonView.RPC("PlaceObstacle", RpcTarget.MasterClient, yPosition);
                } else if (chance < israChance + wallChance)
                {
                    this.photonView.RPC("PlaceWallObstacle", RpcTarget.MasterClient);
                } else if (chance < israChance + wallChance + singleChance)
                {
                    this.photonView.RPC("PlaceObstacle", RpcTarget.MasterClient, yPosition);
                }

                obstacleDelayTime = FlappyRoyaleGameManager.Instance.actualDifficulty.obstacleDelay;
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
    public void PlaceObstacle(int yPosition)
    {
        Vector3 newSpawnPosition = spawnPosition.position + new Vector3(0, yPosition);
        GameObject obstacleGo = ObjectPooler.Instance.SpawnFromPool("SingleObstacle", newSpawnPosition, Quaternion.identity);
        Obstacle obstacleController = obstacleGo.GetComponent<Obstacle>();
        obstacleController.Show();
    }

    [PunRPC]
    private void PlaceWallObstacle()
    {
        GameObject obstacleGo = ObjectPooler.Instance.SpawnFromPool("WallObstacle", wallSpawnPoint.position, wallSpawnPoint.rotation);
        WallObstacle wallController = obstacleGo.GetComponent<WallObstacle>();
        wallController.Show();
        float chance = Random.value;
        if (chance < 0.25)
        {
            wallController.EnableToClose();
            wallController.CloseLeft();
        } else if (chance < 0.5)
        {
            wallController.EnableToClose();
            wallController.CloseRight();
        } else if (chance < 0.75)
        {
            wallController.DisableToClose();
            wallController.CloseLeft();
        } else if (chance < 1)
        {
            wallController.DisableToClose();
            wallController.CloseRight();
        }

    }
    private void TestPlaceObstacle(GameObject obstacle, int yPosition)
    {
        Vector3 newSpawnPosition = spawnPosition.position + new Vector3(0, yPosition);
        GameObject obstacleGo = GameObject.Instantiate(obstacle, newSpawnPosition, Quaternion.identity, transform);
        StartCoroutine(destroyObstacle(obstacleGo));
    }
    private void TestPlaceWallObstacle(GameObject obstacle)
    {
        GameObject obstacleGo = GameObject.Instantiate(obstacle, wallSpawnPoint.position, wallSpawnPoint.rotation);
        WallObstacle wallController = obstacleGo.GetComponent<WallObstacle>();
        wallController.DisableToClose();
    }
    public IEnumerator destroyObstacle(GameObject obstacle)
    {
        yield return new WaitForSeconds(5f);
        Destroy(obstacle);
    }
}