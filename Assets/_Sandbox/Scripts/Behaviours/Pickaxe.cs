using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    public bool inRockRange;

    public void ActionButton()
    {
        Debug.Log("Pickaxe Dude");
    }


    public void HoldActionButton()
    {
        //if is near rock
            //transform position towards rock
            //
        if(inRockRange)
        {
            Debug.Log("Picakxe Held");
        }
        
    }
}
