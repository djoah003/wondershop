using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private Light _light;
    public GameObject avatar;
    
    // Start is called before the first frame update
    private void OnDrawGizmos()
    { 
        float range = _light.range;
        // spotAngle uses the lights OuterSpotAngle value. Divide it by 2 to be able to use it creating triangles.
        float angle = _light.spotAngle / 2;
        // The beginning of the triangle
        Vector3 start = avatar.transform.position;
        
        Vector3 forwardVector = (start + avatar.transform.forward);
        Vector3.Normalize(forwardVector);
        forwardVector.z *= range;

        Vector3 leftTri = Quaternion.Euler(0, -angle, 0f) * forwardVector;
        Vector3 rightTri = Quaternion.Euler(0, angle, 0f) * forwardVector;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, forwardVector);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, leftTri);
        Gizmos.DrawLine(start, rightTri);
        Gizmos.DrawLine(leftTri, rightTri);
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
