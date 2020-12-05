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
    public RectTransform leftCannon;
    public RectTransform rightCannon;
    public float defaultRightCannonAttackCooldown = 1f;
    public float defaultLeftCannonAttackCooldown = 1f;
    public float fireForce = 1000f;
    public float amountOfAttackItems = 6;

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
                while (i < 30)
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
                Debug.Log("NO MORE ITEMS TO THROW");
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
        Debug.Log($"CHECKING IF ATTACK COOLDOWN <= 0 {leftCannonAttackCooldown}");
        if (leftCannonAttackCooldown <= 0)
        {
            foreach (GameObject item in itemsToThrow)
            {
                FallingItemController itemStats = item.GetComponent<FallingItemController>();
                if (!itemStats.fallingItem.isAttackItem)
                {
                    GameObject newItem = Instantiate<GameObject>(item, leftCannon.position, Quaternion.identity, transform);
                    DragController dragController = newItem.GetComponent<DragController>();
                    dragController.SetCanvas(canvas);
                    Destroy(newItem, 20);
                    Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
                    rigidbody.AddForce(new Vector2(1,1) * fireForce);
                }
            }
            leftCannonAttackCooldown = defaultLeftCannonAttackCooldown;
            return true;
        } else
        {
            return false;
        }
    }
    public bool FireRightCannon()
    {
        Debug.Log($"CHECKING IF ATTACK COOLDOWN <= 0 {rightCannonAttackCooldown}");
        if (rightCannonAttackCooldown <= 0)
        {
            foreach (GameObject item in itemsToThrow)
            {
                FallingItemController itemStats = item.GetComponent<FallingItemController>();
                if (!itemStats.fallingItem.isAttackItem)
                {
                    GameObject newItem = Instantiate<GameObject>(item, rightCannon.position, Quaternion.identity, transform);
                    DragController dragController = newItem.GetComponent<DragController>();
                    dragController.SetCanvas(canvas);
                    Destroy(newItem, 20);
                    Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
                    rigidbody.AddForce(new Vector2(-1, 1) * fireForce);
                }
            }
            rightCannonAttackCooldown = defaultRightCannonAttackCooldown;
            return true;
        } else
        {
            return false;
        }
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
