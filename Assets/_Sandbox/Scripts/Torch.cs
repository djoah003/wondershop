using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Torch : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private GameObject avatar;
    [Range(1, 10)] [SerializeField] private int raySegmentCount;
   
    
    private float range => light.range;
    Vector3 start => light.transform.position;
    float angle => light.spotAngle / 2;

    
    // Start is called before the first frame update
    private void OnDrawGizmos()
    { 
        // spotAngle uses the lights OuterSpotAngle value. Divide it by 2 to be able to use it creating triangles.

        Vector3 lightForward = light.transform.forward;
        Vector3 lightMinAngle = Quaternion.Euler(0, -angle, 0) * lightForward;
        Vector3 lightMaxAngle = Quaternion.Euler(0, angle, 0) * lightForward;
        float lightAngle = Vector3.Angle(lightMinAngle, lightForward);
        
        for (int i = 0; i < raySegmentCount; i++)
        {
            float rayAngle = (i / (float)raySegmentCount) * lightAngle;
            Gizmos.DrawRay(start,  Quaternion.Euler(0, rayAngle, 0) * lightMinAngle * range);
            Gizmos.DrawRay(start,  Quaternion.Euler(0, rayAngle, 0) * lightForward * range);
        }
        Gizmos.DrawRay(start,  lightMaxAngle * range);

    }

    void TorchLogic()
    {
        Vector3 lightForward = light.transform.forward;
        Vector3 lightMinAngle = Quaternion.Euler(0, -angle, 0) * lightForward;
        Vector3 lightMaxAngle = Quaternion.Euler(0, angle, 0) * lightForward;
        float lightAngle = Vector3.Angle(lightMinAngle, lightForward);
        
        for (int i = 0; i < raySegmentCount; i++)
        {
            float rayAngle = (i / (float)raySegmentCount) * lightAngle;
            Gizmos.DrawRay(start,  Quaternion.Euler(0, rayAngle, 0) * lightMinAngle * range);
            Gizmos.DrawRay(start,  Quaternion.Euler(0, rayAngle, 0) * lightForward * range);
        }
        Gizmos.DrawRay(start,  lightMaxAngle * range);
    }
    

    void Start()
    {
        avatar = transform.parent.root.Find("Avatar").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
