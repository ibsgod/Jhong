using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayspin : MonoBehaviour
{
    // Start is called before the first frame update
    float origY;
    int goingUp = 1;
    void Start()
    {
        origY = transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, 150 * Time.deltaTime, Space.World);
        transform.Translate(0, 0.8f * Time.deltaTime * goingUp, 0);
        if (transform.position.y > origY + 0.2) {
            goingUp = -1;
        }
        if (transform.position.y < origY - 0.2) {
            goingUp = 1;
        }


    }
}
