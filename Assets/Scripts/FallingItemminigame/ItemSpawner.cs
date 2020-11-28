using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<float> spawnPositions;
    public GameObject[] itemsToThrow;
    public float defaultDelayTime;
    private float delayTime;
    public List<float> usedSpawnPositions;
    public Canvas canvas;
    public RectTransform leftCannon;
    public RectTransform rightCannon;
    public float fireForce = 1000f;
    public float amountOfAttackItems = 6;
    private Queue<GameObject> itemsPool;
    public void Start()
    {
        delayTime = defaultDelayTime;
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
        if (itemsPool.Count > 0)
        {
            if (delayTime <= 0f)
            {
                DropRandom();
            }
            delayTime -= Time.deltaTime;
            delayTime = Mathf.Clamp(delayTime, 0f, Mathf.Infinity);
        } else
        {
            Debug.Log("NO MORE ITEMS TO THROW");
        }
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
        int spawnIndex = Random.Range(0, spawnPositions.Count);
        Vector3 newSpawnPosition = new Vector3(transform.position.x + spawnPositions[spawnIndex], transform.position.y);
        GameObject newItem = Instantiate<GameObject>(item, newSpawnPosition, Quaternion.identity, transform);
        DragController dragController = newItem.GetComponent<DragController>();
        dragController.SetCanvas(canvas);
        Destroy(newItem, 20);
        delayTime = defaultDelayTime;
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
    public void FireLeftCannon()
    {
        foreach (GameObject item in itemsToThrow)
        {
            GameObject newItem = Instantiate<GameObject>(item, leftCannon.position, Quaternion.identity, transform);
            DragController dragController = newItem.GetComponent<DragController>();
            dragController.SetCanvas(canvas);
            Destroy(newItem, 20);
            Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(new Vector2(1,1) * fireForce);
        }
    }
    public void FireRightCannon()
    {
        foreach (GameObject item in itemsToThrow)
        {
            GameObject newItem = Instantiate<GameObject>(item, rightCannon.position, Quaternion.identity, transform);
            DragController dragController = newItem.GetComponent<DragController>();
            dragController.SetCanvas(canvas);
            Destroy(newItem, 20);
            Rigidbody2D rigidbody = newItem.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(new Vector2(1, 1) * fireForce);
        }
    }
}
