using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    public List<float> spawnPositions;
    public GameObject[] itemsToThrow;
    public float defaultDelayTime;
    public Canvas canvas;
    
    [Header("Attacking Cannons Configuration")]
    public Cannon leftCannon;
    public Cannon rightCannon;
    public float defaultRightCannonAttackCooldown = 1f;
    public float defaultLeftCannonAttackCooldown = 1f;
    public float fireForce = 1000f;
    public float amountOfAttackItems = 6;
    public int multiplyItems = 30;
    private List<float> usedSpawnPositions;
    private float delayTime;
    private Queue<GameObject> itemsPool;
    private float leftCannonAttackCooldown;
    private float rightCannonAttackCooldown;
    private bool spawningFlag = false;
    public bool DoneSpawning { get; private set; }
    public void Start()
    {
        delayTime = defaultDelayTime;
        leftCannonAttackCooldown = defaultLeftCannonAttackCooldown;
        rightCannonAttackCooldown = defaultRightCannonAttackCooldown;
        leftCannon.Setup(itemsToThrow, canvas);
        rightCannon.Setup(itemsToThrow, canvas);
    }
    public void Awake()
    {
        List<GameObject> randomizedItemsToThrow = new List<GameObject>();
        foreach (GameObject item in itemsToThrow)
        {
            var i = 0;
            FallingItemController controller = item.GetComponent<FallingItemController>();
            
            if (controller.fallingItem.isAttackItem)
            {
                while (i < amountOfAttackItems)
                {
                    randomizedItemsToThrow.Add(item);
                    i++;
                }
            } else
            {
                while (i < multiplyItems)
                {
                    randomizedItemsToThrow.Add(item);
                    i++;
                }
            }
        }
        Shuffle(randomizedItemsToThrow);
        itemsPool = new Queue<GameObject>(randomizedItemsToThrow);
    }
    public void Update()
    {
        if (spawningFlag && !DoneSpawning)
        {
            if (itemsPool.Count > 0 && !OnlyAttackItemsLeft())
            {
                if (delayTime <= 0f)
                {
                    DropRandom();
                }
                delayTime -= Time.deltaTime;
                delayTime = Mathf.Clamp(delayTime, 0f, Mathf.Infinity);
            } else
            {
                StartCoroutine(FinishSpawning());
            }
            if (leftCannonAttackCooldown > 0)
            {
                leftCannonAttackCooldown -= Time.deltaTime;
                leftCannonAttackCooldown = Mathf.Clamp(leftCannonAttackCooldown, 0f, Mathf.Infinity);
            }
            if (rightCannonAttackCooldown > 0)
            {
                rightCannonAttackCooldown -= Time.deltaTime;
                rightCannonAttackCooldown = Mathf.Clamp(rightCannonAttackCooldown, 0f, Mathf.Infinity);
            }
        } else if (OnlyAttackItemsLeft())
        {
            if (itemsPool.Count > 0)
            {
                if (delayTime <= 0f)
                {
                    DropRandom();
                }
                delayTime -= Time.deltaTime;
                delayTime = Mathf.Clamp(delayTime, 0f, Mathf.Infinity);
            }
        }
    }
    private IEnumerator FinishSpawning()
    {
        yield return new WaitForSeconds(4f);
        int mainPlayerId = GameManager.Instance.GetMainPlayer().playerStats.id;
        FallingGameManager.Instance.photonView.RPC("SetSpawnerDone", Photon.Pun.RpcTarget.All, mainPlayerId);
        DoneSpawning = true;
    }
    private void DropAllAtOnce()
    {
        foreach (GameObject item in itemsToThrow)
        {
            int spawnIndex = Random.Range(0, spawnPositions.Count);
            Vector3 newSpawnPosition = new Vector3(transform.position.x + spawnPositions[spawnIndex], transform.position.y);
            GameObject newItem = Instantiate<GameObject>(item, newSpawnPosition, Quaternion.identity, transform);
            DragController dragController = newItem.GetComponent<DragController>();
            dragController.SetCanvas(canvas);
            Destroy(newItem, 20);
            usedSpawnPositions.Add(spawnPositions[spawnIndex]);
            spawnPositions.Remove(spawnPositions[spawnIndex]);
        }
        spawnPositions = new List<float>(usedSpawnPositions);
        usedSpawnPositions.Clear();
        delayTime = defaultDelayTime;
    }
    private void DropRandom()
    {
        GameObject item = itemsPool.Dequeue();
        FallingItemController itemInfo = item.GetComponent<FallingItemController>();
        if (itemInfo.fallingItem.isAttackItem && (FallingGameManager.Instance.GetMostPoints().playerId == GameManager.Instance.GetMainPlayer().playerStats.id))
        {
            itemsPool.Enqueue(item);
        } else
        {
            int spawnIndex = Random.Range(0, spawnPositions.Count);
            Vector3 newSpawnPosition = new Vector3(transform.position.x + spawnPositions[spawnIndex], transform.position.y);
            GameObject newItem = Instantiate<GameObject>(item, newSpawnPosition, Quaternion.identity, transform);
            DragController dragController = newItem.GetComponent<DragController>();
            dragController.SetCanvas(canvas);
            Destroy(newItem, 20);
            delayTime = defaultDelayTime;
        }
    }
    private void Shuffle(List<GameObject> objects)
    {
        GameObject aux;
        for (int i = objects.Count-1; i > 0; i--)
        {
            int newIndex = Random.Range(0, i);
            aux = objects[newIndex];
            objects[newIndex] = objects[i];
            objects[i] = aux;
        }
    }
    public bool FireLeftCannon()
    {
        if (leftCannonAttackCooldown <= 0)
        {
            leftCannon.AnimateFire();
            leftCannonAttackCooldown = defaultLeftCannonAttackCooldown;
            return true;
        } else
        {
            return false;
        }
    }
    public void TestFireLeftCannon()
    {
        leftCannon.AnimateFire();
        leftCannonAttackCooldown = defaultLeftCannonAttackCooldown;
    }
    public bool FireRightCannon()
    {
        if (rightCannonAttackCooldown <= 0)
        {
            rightCannon.AnimateFire();
            rightCannonAttackCooldown = defaultRightCannonAttackCooldown;
            return true;
        } else
        {
            return false;
        }
    }
    public void TestFireRightCannon()
    {

        rightCannon.AnimateFire();
        rightCannonAttackCooldown = defaultRightCannonAttackCooldown;
    }
    public void Activate()
    {
        spawningFlag = true;
    }
    public bool OnlyAttackItemsLeft()
    {
        if (itemsPool.ToList().Find(o => !o.GetComponent<FallingItemController>().fallingItem.isAttackItem))
        {
            return false;
        }
        else 
        { 
            return true; 
        };
    }
}
