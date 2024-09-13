using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class evade : InputManager
{
    // Start is called before the first frame update
    bool evading;

    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        // if ((player.transform.position - transform.position).magnitude < 2) {
        //     evading = true;
        // }
        // if ((player.transform.position - transform.position).magnitude > 4) {
        //     evading = false;
        // }
        // dancing = !evading;
        // running = evading;
        // if (evading) {
        base.resetInputs();
        GameObject target = player;
        inputs["attack"] = 1;
        if (GameObject.Find("speardisplay") != null) {
            target = GameObject.Find("speardisplay");
            // inputs["fire"] = 1;
        }
            Vector3 direction = transform.position - target.transform.position;
            Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            lookAtAngle.y += 180;
            if (180 > transform.eulerAngles.y % 360 - lookAtAngle.y % 360 && transform.eulerAngles.y % 360 - lookAtAngle.y % 360 > 0 ||
            transform.eulerAngles.y % 360 - lookAtAngle.y % 360 < -180) {
                inputs["left"] = 2;
                inputs["right"] = 0;
            }
            else {
                inputs["right"] = 2;
                inputs["left"] = 0;
            }
            inputs["forward"] = 2;
        // }
    }
}
