using System;
using System.Collections;
using System.Collections.Generic;
using Scene.InteractiveItems;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class LanternDown : MonoBehaviour
{
    public StairControl aim;
    public List<int> controlList;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(-90,0,0);
        gameObject.SetActive(false);
    }

    
    public void Down()
    {
        if(!aim.isCanClick)return;
        aim.isCanClick = false;
        aim.GetComponent<AudioSource>().Play();
        foreach (var id in controlList)
        {
            aim.FlipByID(id);
        }

        StartCoroutine(Unlock());
        aim.JudgeAllRight();
    }

    //解锁可交互
    public IEnumerator Unlock()
    {
        yield return new WaitForSeconds(1.2f);
        aim.isCanClick = true;
    }
    
}
