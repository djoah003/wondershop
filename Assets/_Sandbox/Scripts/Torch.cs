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

    [Range(0, 360)]
    public float rotationValueTest;
    // Start is called before the first frame update

    private void OnDrawGizmos()
    { 
        float range = _light.range;
        float angle = _light.spotAngle;
        Vector3 start = _light.transform.position;
        Vector3 f = _light.transform.forward;
        
        Vector3 forwardVector = (start + f);
        Vector3.Normalize(forwardVector);
        forwardVector.z *= range;
       
        Vector3 vertexB = start + Quaternion.Euler(0, angle / 2, 0f) * Vector3.forward * range;
        Vector3 vertexC = start + Quaternion.Euler(0, -angle / 2, 0f) * Vector3.forward * range;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, forwardVector);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, vertexB);
        Gizmos.DrawLine(start, vertexC);
        Gizmos.DrawLine(vertexC, vertexB);
        
    }
    
    Vector3 RotateVector(Vector3 originalVector, Vector3 rotationAxis, float rotationValue)
    {
        // Create a rotation quaternion
        Quaternion rotationQuaternion = Quaternion.AngleAxis(rotationValue, rotationAxis);
        // Rotate the vector
        Vector3 rotatedVector = rotationQuaternion * originalVector;

        return rotatedVector;
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
