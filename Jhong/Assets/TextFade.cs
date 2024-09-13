using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextFade : MonoBehaviour
{
    public Camera thecamera;
    public Transform startPos;
    Vector3 pos;
    public float fadeTime = 2;
    Vector3 offset;
    float startTime;
    void Start()
    {
        startTime = Time.time;
        offset = new Vector3(Random.Range(-.1f, .1f), Random.Range(0f, .2f), Random.Range(-.1f, .1f));
    }

    void Update()
    {
        if (startPos) {
            pos = startPos.position;
        }
        transform.position = thecamera.WorldToScreenPoint(pos + offset + new Vector3(0, (Time.time - startTime)/2, 0));
        transform.localScale /= 1.01f;
        Color currColour = GetComponent<TextMeshProUGUI>().color;
        GetComponent<TextMeshProUGUI>().color = new Color(currColour.r, currColour.g, currColour.b, 1 - (Time.time - startTime) / fadeTime);
        if (Time.time - startTime >= fadeTime) {
            Destroy(this.gameObject);
        }
    }
}
