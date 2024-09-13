using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DogBehaviour : EntityBehaviour
{
    public GameObject player;
    new void Start()
    {
        base.Start();
        worldScript = GameObject.Find("WorldObject").GetComponent<WorldScript>();
        player = GameObject.Find("player").transform.Find("Center").gameObject;
    }

    new void Update()
    {
        base.Update();
        transform.parent = null;
        transform.LookAt(player.transform.position);

        if ((transform.position - (player.transform.position + new Vector3(0, 0, -2))).magnitude > 4) {
            GetComponent<Rigidbody>().velocity = (player.transform.position + new Vector3(0, 0, -2) - transform.position).normalized * 4;
        }
        else {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    
}
