using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSpawner : MonoBehaviour
{
    [Header("Spawning")] public int maxConcurrent;
    public int minConcurrent;
    public float spawnInSeconds;

    [Header("Despawning")] [SerializeField]
    private bool despawn;

    [SerializeField] private bool despawnCancelOnMove;
    [SerializeField] private float despawnInSeconds;
    [SerializeField] [Range(0, 10)] private float despawnRandomRange;
    
    private float _previousSpawnTimestamp;
    public List<GameObject> items = new List<GameObject>();
    private readonly List<Vector3> _positions = new List<Vector3>();
    private readonly Queue<GameObject> _cooldown = new Queue<GameObject>();
    private readonly List<GameObject> _removedItems = new List<GameObject>();

    public int GetRemovedItemsCount() => _removedItems.Count;

    private void Awake()
    {
        // add all items and their original position
        foreach (Transform child in transform) NewItem(child);
        // check if maxConcurrent is set
        if (maxConcurrent == 0) maxConcurrent = items.Count;
        // check that minConcurrent is not too large
        if (items.Count < minConcurrent) minConcurrent = items.Count;
    }

    private void NewItem(Transform item)
    {
        items.Add(item.gameObject);
        _positions.Add(item.localPosition);
    }

    private void OnEnable()
    {
        _previousSpawnTimestamp = Time.time;
        foreach (GameObject item in items) item.SetActive(false);
    }

    private void OnDisable()
    {
        _cooldown.Clear();
        foreach (GameObject item in items)
        {
            item.SetActive(false);
            Destroy(item.GetComponent<RandomDespawn>());
        }
    }

    private void Update()
    {
        // check timer based spawning condition
        bool timerBased = _previousSpawnTimestamp + spawnInSeconds < Time.time;
        // check count based spawning condition
        int activeCount = items.Count(item => item.activeSelf);
        bool countBased = activeCount < minConcurrent || activeCount < maxConcurrent;
        if (timerBased && countBased) Spawn();
    }

    private void Spawn()
    {
        // get the next available object
        List<GameObject> inactive = items.Where(item => !item.activeSelf && !_cooldown.Contains(item) && !_removedItems.Contains(item)).ToList();
        // if items are at max capacity, start emptying the cooldown queue
        if (_cooldown.Count >= maxConcurrent) _cooldown.Dequeue();
        // if not available objects, return early
        if (inactive.Count == 0) return;
        // otherwise draft random item from list
        int randomIndex = Random.Range(0, inactive.Count);
        GameObject item = inactive[randomIndex];
        // set item as active
        item.SetActive(true);
        // spawn item in original position
        if (_positions.Count != 0) item.transform.localPosition = _positions[items.IndexOf(item)];
        // track spawner timer
        _previousSpawnTimestamp = Time.time;
        // if despawning is enabled
        if (!despawn) return;
        // add random despawner
        item.AddComponent<RandomDespawn>().Initialize(Random.Range(despawnInSeconds, despawnInSeconds + despawnRandomRange), null, despawnCancelOnMove);
        // and add the item for cooldown list to prevent it spawning one after another
        _cooldown.Enqueue(item);
    }

    public void Refresh()
    {
        _cooldown.Clear();
        _previousSpawnTimestamp = Time.time;
        foreach (GameObject item in items)
        {
            item.SetActive(false);
            Destroy(item.GetComponent<RandomDespawn>());
        }
    }

    public void RemoveItemFromRotation(GameObject item)
    {
        Destroy(item.GetComponent<RandomDespawn>());
        item.SetActive(false);
        _removedItems.Add(item);
    }
    
    public void AddItemToRotation()
    {
        _removedItems.RemoveAt(Random.Range(0, _removedItems.Count));
    }
}