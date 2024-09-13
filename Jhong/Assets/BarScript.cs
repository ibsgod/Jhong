using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {

    public Slider slider;
    // public Gradient gradient;
    public Image fill;
    public float maxVal;
    public float val;

    public void setMaxValue(float val)
    {
        this.maxVal = val;
        slider.maxValue = val;
        // fill.color = gradient.Evaluate(1f);

    }
    public void setValue(float val)
    {
        this.val = Mathf.Max(0, Mathf.Min(val, maxVal));
        slider.value = this.val;
        // fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
