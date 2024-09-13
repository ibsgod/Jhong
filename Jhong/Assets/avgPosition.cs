using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avgPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> objs = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avgPosition = Vector3.zero;
        float minx = 99999;
        float minz = 99999;
        float maxx = -99999;
        float maxz = -99999;
        for (int i = 0; i < objs.Count; i++) {
            avgPosition += objs[i].transform.position;
            minx = Mathf.Min(minx, objs[i].transform.position.x);
            minz = Mathf.Min(minz, objs[i].transform.position.z);
            maxx = Mathf.Max(maxx, objs[i].transform.position.x);
            maxz = Mathf.Max(maxz, objs[i].transform.position.z);

        }
        float range = Mathf.Sqrt(Mathf.Pow((maxx - minx), 2) + Mathf.Pow((maxz - minz), 2));
        avgPosition /= objs.Count;
        avgPosition.y = range * 0.6f;
        transform.position = avgPosition;
        transform.eulerAngles = new Vector3(range * 1.4f , 0, 0);
    }
}
