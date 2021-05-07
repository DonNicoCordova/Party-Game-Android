using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ObjectPooler : GenericDestroyableSingletonClass<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag(pool.tag);
            foreach(GameObject obstacle in obstacles)
            {
                objectPool.Enqueue(obstacle);
            }
            for (int i= objectPool.Count; i< pool.size; i++)
            {
                GameObject obstacleGo = PhotonNetwork.Instantiate(pool.prefab.name, transform.position + new Vector3(0, -20, 0), Quaternion.identity);

                
                objectPool.Enqueue(obstacleGo);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"POOL WITH TAG {tag} DOESNT EXISTS");
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}
