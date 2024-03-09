using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnItems : MonoBehaviour
{
    private Transform[] _spawnPoint;
    private readonly List<int> _freeIndexes = new();

    private void Awake()
    {
        _spawnPoint = new Transform[transform.childCount];
        int counter = 0;
        foreach (Transform child in transform)
        {
            _spawnPoint[counter] = child.transform;
            _freeIndexes.Add(counter);
            counter += 1;
        }
    }

    public bool SpawnInOrderedPoint(GameObject prefab)
    {
        if (_spawnPoint.Length == 0 || _freeIndexes.Count == 0) return false;
        int spawnPoint = _freeIndexes.First();
        Instantiate(prefab, _spawnPoint[spawnPoint]);
        _freeIndexes.RemoveAt(0);
        return true;
    }

    public bool SpawnInRandomPoint(GameObject prefab)
    {
        if (_spawnPoint.Length == 0 || _freeIndexes.Count == 0) return false;
        int index = Random.Range(0, _freeIndexes.Count);
        int spawnPoint = _freeIndexes[index];
        Instantiate(prefab, _spawnPoint[spawnPoint]);
        _freeIndexes.RemoveAt(index);
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Transform child in transform) Gizmos.DrawWireSphere(child.position, radius: 1f);
    }
}