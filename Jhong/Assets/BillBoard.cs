using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour {
    GameObject thecamera;
    private void Start()
    {
        thecamera = GameObject.FindWithTag("MainCamera");
    }
    

    void LateUpdate()
    {
        transform.LookAt(thecamera.transform.forward + transform.position);
    }
}
