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
    [Range(1, 100)] [SerializeField] private int raySegmentCount;
    [SerializeField] private float lightDamage = 0.01f;
    
    private float range => light.range;
    private Vector3 start => light.transform.position;
    private Vector3 lightForward => light.transform.forward;
    private float angle => light.spotAngle / 2;
    private void FixedUpdate() => TorchLogic();

    void TorchLogic()
    {
        Vector3 lightMinAngle = Quaternion.Euler(0, -angle, 0) * lightForward;
        Vector3 lightMaxAngle = Quaternion.Euler(0, angle, 0) * lightForward;
        float lightAngle = Vector3.Angle(lightMinAngle, lightForward);
        
        // LEFT SIDE
        for (int i = 0; i < raySegmentCount; i++)
        {
            float rayAngle = (i / (float)raySegmentCount) * lightAngle;
            if (Physics.Raycast(start, Quaternion.Euler(0, rayAngle, 0) * lightMinAngle, out RaycastHit leftHit, range))
                DrawLightRay(rayAngle, lightMinAngle, leftHit.distance, leftHit);
            else
                DrawLightRay(rayAngle, lightMinAngle, range);
        }
        
        // RIGHT SIDE 
        for (int i = 0; i < raySegmentCount + 1; i++)
        {
            float rayAngle = (i / (float)raySegmentCount) * lightAngle;
            if (Physics.Raycast(start, Quaternion.Euler(0, -rayAngle, 0) * lightMaxAngle, out RaycastHit rightHit, range))
                DrawLightRay(-rayAngle, lightMaxAngle, rightHit.distance, rightHit);
            else
                DrawLightRay(-rayAngle, lightMaxAngle, range);
        }
    }
    void DrawLightRay(float rayAngle, Vector3 rayDirection, float rayRange, RaycastHit hit = default)
    {
        Debug.DrawRay(start, Quaternion.Euler(0, rayAngle, 0) * rayDirection * rayRange);
        // Check for hit
        if (hit.rigidbody != null && hit.rigidbody.gameObject.CompareTag("ShadowMonster"))
            OnShadowMonsterHit(hit);
    }
    void OnShadowMonsterHit(RaycastHit hit) => hit.rigidbody.gameObject.SendMessage("TakeDamage", lightDamage);
}
