using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerainput : MonoBehaviour
{
public Transform camPos;
public float input;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
          input= Input.GetAxis("Mouse ScrollWheel");
          
    }
}
