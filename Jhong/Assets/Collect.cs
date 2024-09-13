using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    public float startTime;
    public GameObject player;
    void Start()
    {
        startTime = Time.time;
        player = GameObject.Find("player");
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void Update()
    {
        if (Time.time - startTime > 1) {
            Vector3 direction = transform.position - player.transform.Find("Center").position;
            GetComponent<Rigidbody>().velocity = -direction.normalized * Random.Range(2, 18);
            GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(2, 18), Random.Range(2, 18), Random.Range(2, 18));
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }
    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject == player) {
            player.GetComponent<EntityBehaviour>().collectCoin(gameObject);
        }
        if (collision.gameObject.tag == "Ground") {
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == player) {
            player.GetComponent<EntityBehaviour>().collectCoin(gameObject);
        }
    }
}
