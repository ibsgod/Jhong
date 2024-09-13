using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    float moveTime;     
    public bool moveX;
    public bool moveY;
    public bool moveZ;
    public float moveDir = 0.05f;
    void Start()
    {
        moveTime = Time.time - 2f;
    }

    void Update()
    {
        if (Time.time - moveTime > 4) {
            moveDir *= -1;
            moveTime = Time.time;
        }
        if (moveX) {
            transform.Translate(moveDir * Time.deltaTime, 0, 0);
        }
        if (moveY) {
            transform.Translate(0, moveDir * Time.deltaTime, 0);
        }
        if (moveZ) {
            transform.Translate(0, 0, moveDir * Time.deltaTime);
        }
    }
}
