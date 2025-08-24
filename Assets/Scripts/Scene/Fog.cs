using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frame;
public class Fog : MonoBehaviour
{
    private ParticleSystem[] fogSystem;
    private Transform Cap;

    private void Start()
    {
        fogSystem = gameObject.GetComponentsInChildren<ParticleSystem>();
        Cap = gameObject.GetComponentInChildren<CapsuleCollider>().transform;
        
        
    }

    public void StartFog()
    {
        Cap.localPosition = Vector3.zero;
        Cap.parent = GlobalInstance.Player().transform;
        Cap.localPosition = Vector3.zero;
        for (int i = 0; i < fogSystem.Length; i++)
        {
            fogSystem[i].Play();
        }
    }

    public void StopFog()
    {
        for (int i = 0; i < fogSystem.Length; i++)
        {
            fogSystem[i].Stop();
        }

        Cap.transform.position = Vector3.zero;
    }
    
}
