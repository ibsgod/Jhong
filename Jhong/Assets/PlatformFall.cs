using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    public GameObject player;
    float landStart = -1;
    Vector3 origPos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        origPos = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.position.y < -10) {
            Destroy(transform.parent.gameObject);
        }
        if (landStart > 0) {
            if (Time.time - landStart < 5) {
                transform.parent.position = origPos + new Vector3(Random.Range(-0.02f, 0.02f), 0, Random.Range(-0.02f, 0.02f));
            }
            else {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject == player && landStart == -1) {
            landStart = Time.time;
        }
    }
}
