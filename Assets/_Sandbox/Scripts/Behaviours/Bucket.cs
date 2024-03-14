using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
   [SerializeField]
   private int currentGold;
   [SerializeField]
   private int maxGold = 5;

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
            }
        }
        
    }

   
   
}
