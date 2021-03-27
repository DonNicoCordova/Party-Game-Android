using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private TMPro.TextMeshProUGUI counter;

    private void Start()
    {
        counter = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public void SetMaxTime(float maxTime)
    {
        slider.maxValue = maxTime;
        slider.value = maxTime;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetTimeLeft(float timeLeft)
    {
        if (fill != null && slider != null)
        {
            counter.text = $"{timeLeft:0} seg.";
            slider.value = timeLeft;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
