using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeControl : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed",0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            animator.SetFloat("Speed",1);
        }
    }
    
}
