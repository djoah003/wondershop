using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("pickaxePlayer"))
        {
            Debug.Log("Enter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("pickaxePlayer"))
        {
            Debug.Log("Exit");
        }
    }
}
