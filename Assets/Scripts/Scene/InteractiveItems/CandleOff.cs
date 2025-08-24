using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleOff : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private Vector3 oriPos;
    // Start is called before the first frame update
    void Start()
    {
        oriPos = transform.position;
        particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void putOut() {
        ParticleSystem.MainModule main = particleSystem.main;
        main.loop = false;
        main.startLifetime = 2f;
        main.startSize = 2f;
        main.startSpeed = 2f;
        main.startColor = Color.black;
    }
    public void putOn()
    {
        particleSystem.Play();
        transform.position = oriPos + new Vector3(0, 1, 0);
        particleSystem.transform.localScale *= -1;
        ParticleSystem.MainModule main = particleSystem.main;
        main.loop = true;
        main.startLifetime = 2f;
        main.startSize = 5f;
        main.startSpeed = 4f;
        main.startColor = new Color(227f, 128f, 12f);

    }
}
