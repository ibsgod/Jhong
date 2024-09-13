using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SpellBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject owner;
    public AudioSource audioSource;
    public AudioClip firesfx;
    public AudioClip hitsfx;
    float startTime;
    public float activateTime;
    bool activated = false;
    public float lifeTime = 3;
    public float dmg;
    UnityEngine.Vector3 dir;
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (owner) {
            dir = owner.transform.forward;
        }
        if (Time.time - startTime > 4)  {
            Destroy(gameObject);
        }
        // if (Time.time - startTime > activateTime && !activated) {
        if (owner != null && 
            (owner.name == "player" && owner.GetComponent<InputManager>().inputs["fire"] == 3 && !activated||
            owner.name != "player" && Time.time - startTime > activateTime && !activated)) {
            audioSource.PlayOneShot(firesfx);
            activated = true;
            if (owner) {
                transform.rotation = owner.transform.rotation;
                transform.Rotate(0, 270, 90);
            }
            GetComponent<Rigidbody>().AddForce(dir * 600);
        }
        if (owner != null && owner.name == "player" && owner.GetComponent<InputManager>().inputs["fire"] == 2 && !activated) {
            transform.localScale += new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
