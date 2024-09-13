using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 prevPos;
    public Vector3 vel;
    public GameObject owner;
    public float dmg = 2;

    void Start()
    {
        prevPos = transform.Find("Cone/Tip").position;
        vel = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        vel = transform.Find("Cone/Tip").position - prevPos;
        prevPos = transform.Find("Cone/Tip").position;
        // print(vel);
    }

    // private void OnTriggerEnter(Collider other) {
    //     if (other.gameObject.tag == "Ground") {
    //         GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
    //     }
    // }
    // private void OnTriggerExit(Collider other) {
    //     if (other.gameObject.tag == "Ground") {
    //         GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    //     }
    // }
}
