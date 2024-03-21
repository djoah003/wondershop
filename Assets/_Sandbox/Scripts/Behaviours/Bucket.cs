using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bucket : MonoBehaviour
{
   public bool itemHeld = true;

   [SerializeField]
   private int currentGold;
   [SerializeField]
   private int maxGold = 5;
   [SerializeField] List<GameObject> gems = new List<GameObject>();


   [Header("Collectible Drop")]
   [SerializeField] private GameObject collectable;
   [SerializeField] private GameObject gold;


    void Start()
    {
        maxGold = 5;
    }
    void OnTriggerEnter(Collider other)
    {
        if(currentGold < maxGold)
        {   
            if(other.CompareTag("gold"))
            {
                Destroy(other);
                currentGold++;
                gems[currentGold - 1].SetActive(true);
            }
           
        }
        Debug.Log(other.name);
        if(other.CompareTag("dropoff") && currentGold > 0)
            {
               
            }
        
    }

    private void DropItem()
    {
        GameObject collectableDrop = Instantiate(collectable, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        DropGold();

    }


    private void DropGold()
    {
        for(int i =0; i < currentGold; i++)
        {
            GameObject collectableGold = Instantiate(gold, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
            collectableGold.GetComponent<Rigidbody>().AddForce(transform.up, ForceMode.Impulse);
            gems[i].SetActive(false);
        }
    }

    private void DropAtBase()
    {
        Debug.Log("Found Dropoff");
        if(currentGold > 0)
        {
            ScoreManager.IncreaseScore(currentGold);
            for(int i =0; i < currentGold; i++)
            {
                gems[i].SetActive(false);
            }
            currentGold = 0;
        }
        
    }
   
}
