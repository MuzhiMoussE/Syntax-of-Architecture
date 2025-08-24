using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMask : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    // Start is called before the first frame update
    private Vector3 _offset = Vector3.back * 5f;
    [SerializeField] private Transform fogContainer;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        fogContainer = GameObject.Find("Fog Container").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q = Quaternion.identity;
        q.SetLookRotation(Camera.main.transform.forward, Camera.main.transform.up);
       // transform.rotation = q ;
       // transform.rotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        transform.position = player.position;
        var main = Camera.main;
        var transform1 = main.transform;
      //  fogContainer.position = transform1.position;
      //  fogContainer.rotation = transform1.rotation;
    }
}
