using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            else if(other.CompareTag("dropoff"))
            {
                DropGold();
            }
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

   
   
}
