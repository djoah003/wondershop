using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class AvatarRoleManager : MonoBehaviour
{
    [SerializeField]
    private enum roles {Normal, Miner, Torch, Collector};


    public void RoleSwitch(Enum role)
    {
        switch (role)
        {
            case roles.Normal:
                Debug.Log("Normal");
            break;
            case roles.Miner:
                Debug.Log("Miner");
            break;
            case roles.Torch:
                Debug.Log("Torch");
            break;
            case roles.Collector:
                Debug.Log("Collector");
            break;

        }
    }


}
