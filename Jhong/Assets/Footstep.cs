using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource src;
    List<GameObject> colliders; 


    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ground") {
            src.PlayOneShot(clip);
        }
    }
}
