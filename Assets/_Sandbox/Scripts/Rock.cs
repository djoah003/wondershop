using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
  
    public int health = 3;

    [SerializeField] private GameObject destroyedRocks;

    [SerializeField] private GameObject smallRock;

    void Start()
    {
        health = Random.Range(2,3);
    }
    public void DropRock()
    {
        GameObject collectableRock = Instantiate(smallRock, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        collectableRock.GetComponent<Rigidbody>().AddForce(transform.up, ForceMode.Impulse);
        DecreaseHealth();
    }

    private void DecreaseHealth()
    {
        --health;
        if(health <= 0)
        {
            Destroy(gameObject);

        }
    }


    

   

}
