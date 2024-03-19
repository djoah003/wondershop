using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShadowMonster : MonoBehaviour
{
    [SerializeField] private float monsterHealth;
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    [Range(0.1f, 100f)][SerializeField] private float damageThresh;

    private const float DistanceThresh = 15f;

    private float receivedDamage;
    
    private bool isRunningAway;
    private NavMeshAgent navMeshAI;

    private Vector3 monsterPos => gameObject.transform.position;
    
    private void TakeDamage(float damage)
    {
        receivedDamage += damage;
    //    Debug.Log("Damage Taken: " + receivedDamage);
    
        if(receivedDamage < damageThresh) return;
        monsterHealth -= damage;

        if (!isRunningAway)
            isRunningAway = true;
    }

    private void SeekPlayer()
    {
        if (isRunningAway) return;
        
        navMeshAI.destination = ClosestPlayer().transform.position;
    }

    private GameObject ClosestPlayer()
    {
        float currentDistance = 0;
        GameObject closestPlayer = null;
        
        foreach (var player in players)
        {
            var playerPos = player.transform.position;
            if (currentDistance < Vector3.Distance(monsterPos, playerPos))
            { 
                currentDistance = Vector3.Distance(monsterPos, playerPos);
                closestPlayer = player;
            }                
        }
        
        return closestPlayer;
    }
    
    private void RunAway()
    {
        // store the starting transform
        Transform startTransform = transform;
		
        //temporarily point the object to look away from the player
        transform.rotation = Quaternion.LookRotation(transform.position - ClosestPlayer().transform.position);
        
        Vector3 runTo = transform.position + transform.forward * DistanceThresh;
        
        // 5 is the distance to check, assumes you use default for the NavMesh Layer name
        NavMesh.SamplePosition(runTo, out NavMeshHit hit, DistanceThresh, 1 << NavMesh.GetAreaFromName("Walkable")); 
        
        // reset the transform back to our start transform
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        // And get it to head towards the found NavMesh position
        navMeshAI.SetDestination(hit.position);
        
        if (DistanceThresh < Vector3.Distance(transform.position, ClosestPlayer().transform.position))
        {
            isRunningAway = false;
            receivedDamage = 0;
        }    
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // TODO: DO STUFF?
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAI = GetComponent<NavMeshAgent>();
    //     Iterate through every player in scene
        foreach (var player in GameObject.FindGameObjectsWithTag("Avatar"))
            players.Add(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunningAway)
            RunAway();
        else if (monsterHealth <= 0 && !isRunningAway)
            Destroy(gameObject);
        else
            SeekPlayer();
    }
}
