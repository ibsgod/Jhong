using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public GameObject player;
    Object coin;
    public PlayerBehaviour playerScript;
    void Start() {
        player = GameObject.Find("player");
        playerScript = player.GetComponent<PlayerBehaviour>();
        coin = Resources.Load("Coin");
    }
    void OnCollisionEnter(Collision other) {
        if (playerScript.worldScript.thrownWeapons.Contains(other.gameObject) && other.gameObject.GetComponent<SpearBehaviour>().owner != gameObject) {
            GameObject attacker = other.gameObject.GetComponent<SpearBehaviour>().owner;
                other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                AudioClip rand = attacker.GetComponent<PlayerBehaviour>().slashes[UnityEngine.Random.Range(0, attacker.GetComponent<PlayerBehaviour>().slashes.Count-1)];
                playerScript.audioSource.PlayOneShot(rand);
                GameObject c = (GameObject) Instantiate(coin, other.transform.position, Quaternion.identity);
            }
    }
}
