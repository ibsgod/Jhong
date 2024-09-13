using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject spear;
    public Vector3 target;


    public void setRot() {
        if (spear) {
            GetComponent<VisualEffect>().SetVector3("initrot", -spear.GetComponent<SpearBehaviour>().vel);  
        }
        GetComponent<VisualEffect>().SetVector3("initpos", target);
    }
}
