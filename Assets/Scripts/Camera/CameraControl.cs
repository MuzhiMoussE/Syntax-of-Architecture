using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> camerasList;

    private const int HighPriority = 120;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CameraMoveTo(String  CMname)
    {
        ResetAllCamera();
        GameObject.Find(CMname).GetComponent<CinemachineVirtualCamera>().Priority = HighPriority;
        //如果相机不在camerasList中，则添加进去
        if (!camerasList.Contains(GameObject.Find(CMname)))
        {
            camerasList.Add(GameObject.Find(CMname));
        }
    }

    public void CameraMoveBackToMain()
    {
        ResetAllCamera();
        GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>().Priority = 20;
    }

    private void ResetAllCamera()
    {
        foreach (var t in camerasList)
        {
            t.GetComponent<CinemachineVirtualCamera>().Priority = 0;
        }
    }
}
