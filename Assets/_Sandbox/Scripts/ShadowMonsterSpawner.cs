using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowMonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject monster;
    [SerializeField] private List<GameObject> monstersList = new List<GameObject>();
    [SerializeField] private int maxEnemyAmount = 5;
    private List<GameObject> players = new List<GameObject>();
    private Vector3 pos => gameObject.transform.position;

    private GameObject ClosestPlayer()
    {
        float currentDistance = 0;
        GameObject closestPlayer = null;
        
        foreach (var player in players)
        {
            var playerPos = player.transform.position;
            if (currentDistance < Vector3.Distance(pos, playerPos))
            { 
                currentDistance = Vector3.Distance(pos, playerPos);
                closestPlayer = player;
            }                
        }
        return closestPlayer;
    }

    private IEnumerator MonsterSpawning()
    {
        yield return new WaitForSeconds(Random.Range(5f, 11f));
        // TODO: Enemy gets instantiated with index and once died it deletes itself from the list.
        // Check if there's space to spawn a new monster
        if (monstersList.Count < maxEnemyAmount)
        {
            // Spawn a new monster and add it to the list
            GameObject newMonster = Instantiate(monster, gameObject.transform);
            monstersList.Add(newMonster);
        }
        // Iterate through the list backwards to remove destroyed monsters
        for (int i = monstersList.Count - 1; i >= 0; i--)
            if (monstersList[i] == null)
                monstersList.RemoveAt(i);
        // Re-run coroutine
        StartCoroutine(MonsterSpawning());
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            players.Add(player);

        StartCoroutine(MonsterSpawning());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
