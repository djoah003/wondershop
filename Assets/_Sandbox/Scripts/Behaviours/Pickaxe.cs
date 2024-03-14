using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using System;

public class Pickaxe : MonoBehaviour
{
    
    
    [Header("Rock Data")]
    private bool inRockRange;
    [SerializeField]
    private GameObject closestRock;
    [SerializeField] private GameObject gold;

    private bool isHeldDown = false;

    
    [Header("Slider Data")]
    [SerializeField]
    private Slider slider;
    
    [Range(1f, 100f)]
    private float mineValue;

    

    void Start()
    {
    }

    public void ActionButton()
    {
        Debug.Log("Pickaxe Dude");
    }


    public void HoldActionButton(bool value)
    {
        //if is near rock
            //transform position towards rock
            //
        if(inRockRange)
        {
            Debug.Log("Hit Rock");
            //this.transform.root.transform.position = closestRock.transform.position;
            startMiningTimer(value);
        }
        
    }

       public void startMiningTimer(bool isMining = false)
    {
        
        if(isMining)
        {
            StopCoroutine(LetGoTimer());

            Debug.Log("mining");
            RockSlider();
            
            StartCoroutine(LetGoTimer());
        }
        else
        {
            Debug.Log("chilling");
        }
    }

    private IEnumerator LetGoTimer()
    {
        yield return new WaitForSeconds(1f);
        isHeldDown = false;
    }


    void Update()
    {
        slider.value = mineValue;
        
        if(isHeldDown)
            startMiningTimer(true);
        else
            startMiningTimer();
    }



    public void RockSlider()
    {
        mineValue += 0.5f;
        if(mineValue >= 100f)
        {
            closestRock.GetComponent<Rock>().DropRock();
            mineValue = 0;
        }
    }

    public void DropRock()
    {
        GameObject collectableRock = Instantiate(gold, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        collectableRock.GetComponent<Rigidbody>().AddForce(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z));
    }



    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("rock"))
        {
            Debug.Log("Enter");
            inRockRange = true;
            closestRock = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("rock"))
        {
            Debug.Log("Exit");
            inRockRange = false;
            closestRock = null;
        }
    }
}
