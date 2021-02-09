using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PathSpawner : MonoBehaviour
{
    public List<GameObject> paths = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();
    public Transform spawnPosition;
    public float defaultDelayTime = 1f;
    private float pathDelayTime;
    private float obstacleDelayTime;
    private static PathSpawner instance;

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
        pathDelayTime = 0f;
        if (paths != null && paths.Count > 0)
        {
            paths = paths.OrderBy(p => p.transform.position.x).ToList();
        }
    }
    private void Update()
    {
        if (pathDelayTime <= 0f)
        {
            GameObject firstPath = paths.First();
            paths.Remove(firstPath);
            MovePath(firstPath);
            paths.Add(firstPath);
            pathDelayTime = defaultDelayTime;
        }
        if (obstacleDelayTime <= 0f)
        {
            PlaceObstacle();
            obstacleDelayTime = 4 * defaultDelayTime;
        }
        pathDelayTime -= Time.deltaTime;
        pathDelayTime = Mathf.Clamp(pathDelayTime, 0f, Mathf.Infinity);
        obstacleDelayTime -= Time.deltaTime;
        obstacleDelayTime = Mathf.Clamp(obstacleDelayTime, 0f, Mathf.Infinity);
    }
    public void MovePath(GameObject path)
    {
        path.transform.position = spawnPosition.position;
    }
    public void PlaceObstacle()
    {
        int randomIndex = Random.Range(0, obstacles.Count);
        GameObject obstacle = Instantiate(obstacles[randomIndex], spawnPosition.position, Quaternion.identity);
        StartCoroutine(destroyObstacle(obstacle));
    }

    public IEnumerator destroyObstacle(GameObject obstacle)
    {
        yield return new WaitForSeconds(5f);
        Destroy(obstacle);
    }
}
