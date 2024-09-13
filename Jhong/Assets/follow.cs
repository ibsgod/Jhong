using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject followObj;
    Vector3 offset;
    void Start()
    {
        offset = this.transform.position - followObj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = followObj.transform.position + offset;
    }
}
