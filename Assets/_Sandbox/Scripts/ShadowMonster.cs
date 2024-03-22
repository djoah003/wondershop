using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ShadowMonster : MonoBehaviour
{
    [SerializeField] private float monsterHealth;
    [SerializeField] private List<GameObject> players = new List<GameObject>();
    [Range(0.1f, 100f)][SerializeField] private float damageThresh;
    [SerializeField] private Transform dropOffPoint;
    
    private const float DistanceThresh = 15f;
    
    private float receivedDamage;
    private bool isRunningAway, isReturningPlayer;
    private NavMeshAgent navMeshAI;

    private Vector3 monsterPos => gameObject.transform.position;
    private float distanceToClosest => Vector3.Distance(monsterPos, ClosestPlayer().transform.position);
    
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
        if (!isRunningAway && !isReturningPlayer)
            navMeshAI.destination = ClosestPlayer().transform.position;
    }

    private GameObject ClosestPlayer()
    {
        float currentDistance = 999;
        GameObject closestPlayer = null;
        
        foreach (var player in players)
        {
            Vector3 playerPos = player.transform.position;
            // If current distance is bigger, get new distance.
            if (currentDistance > Vector3.Distance(monsterPos, playerPos))
            { 
                currentDistance = Vector3.Distance(monsterPos, playerPos);
                closestPlayer = player;
            }                
        }
        
        Debug.DrawLine(monsterPos, closestPlayer.transform.position, Color.red);
        return closestPlayer;
    }

    
    private void RunAway()
    {
        
        if (distanceToClosest > DistanceThresh)
            isRunningAway = false;
                
        // store the starting transform
        Transform startTransform = transform;
        
        //temporarily point the object to look away from the player
        transform.rotation = Quaternion.LookRotation(transform.position - ClosestPlayer().transform.position);
        //Then we'll get the position on that rotation that's multiplyBy down the path (you could set a Random.range
        // for this if you want variable results) and store it in a new Vector3 called runTo
         Vector3 runTo = transform.position + transform.forward * DistanceThresh;
         
         // 5 is the distance to check, assumes you use default for the NavMesh Layer name
         NavMesh.SamplePosition(runTo, out var hit, 5, 1 << NavMesh.GetAreaFromName("Walkable")); 
            
         // reset the transform back to our start transform
         transform.position = startTransform.position;
         transform.rotation = startTransform.rotation;

         // And get it to head towards the found NavMesh position
         navMeshAI.SetDestination(hit.position);
    }

    private void ReturnPlayer()
    {
        navMeshAI.SetDestination(dropOffPoint.position);

        if (navMeshAI.remainingDistance >= 4)
        {
            isReturningPlayer = true;
            ClosestPlayer().transform.position = transform.position + transform.forward.normalized;
            ClosestPlayer().transform.rotation = transform.rotation;
        }
        else
        {
            isReturningPlayer = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        navMeshAI = GetComponent<NavMeshAgent>();
    //     Iterate through every player in scene
        foreach (var player in GameObject.FindGameObjectsWithTag("Avatar"))
            players.Add(player);
        
        dropOffPoint = GameObject.Find("PlayerDropOff").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRunningAway)
            RunAway();
        else if (distanceToClosest <= 1f && !isRunningAway)
        {
            ReturnPlayer();
        }
        else if (monsterHealth <= 0 && !isRunningAway)
            Destroy(gameObject);
        else
            SeekPlayer();
    }
}
