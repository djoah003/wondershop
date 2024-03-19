using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using System;

public class Pickaxe : MonoBehaviour
{
    public bool itemHeld = true;

    
    [Header("Rock Data")]
    [SerializeField]
    private bool inRockRange;
    [SerializeField]
    private GameObject closestRock;
    [SerializeField] private GameObject gold;
    
    [SerializeField] GameObject canvas;
    private bool isHeldDown = false;

    
    [Header("Slider Data")]
    [SerializeField]
    private Slider slider;
    
    [Range(1f, 100f)]
    private float mineValue;

    [Header("Collectible Drop")]
    [SerializeField] private GameObject collectable;

    

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
            canvas.SetActive(true);
            //this.transform.root.transform.position = closestRock.transform.position;
            startMiningTimer(value);
        }
        else
        {
            mineValue = 0;
            canvas.SetActive(false);

        }
        
    }

       public void startMiningTimer(bool isMining = false)
    {
        
        if(isMining)
        {
            StopCoroutine(LetGoTimer());

            RockSlider();
            
            StartCoroutine(LetGoTimer());
        }
        else
        {
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
        mineValue = mineValue-0.5f;
        if(mineValue<1f)
        {
            mineValue = 1f;
        }
        
        if(isHeldDown)
            startMiningTimer(true);
        else
            startMiningTimer();
    }



    public void RockSlider()
    {
        mineValue += 1f;
        if(mineValue >= 100f)
        {
            closestRock.GetComponent<Rock>().DropRock();
            inRockRange = false;
            mineValue = 0;
            
        }
    }
   
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("rock"))
        {
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
            canvas.SetActive(false);
            closestRock = null;
            mineValue = 0;
        }
    }


    private void DropItem()
    {
        GameObject collectableDrop = Instantiate(collectable, new Vector3(transform.position.x + 2, transform.position.y + 5f, transform.position.z), transform.rotation);
        collectableDrop.GetComponent<Rigidbody>().AddForce(-1f,1f,0f);
        //collectableDrop.GetComponent<Rigidbody>().AddForce(transform.right, ForceMode.Force);
    }
}
