using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fill;
    [Range(0,1)] public float value;

    private void Start()
    {
        if (fill == null)
        {
            fill = transform.GetChild(0).GetComponent<Image>();
        }
    }

    private void Update()
    {
        fill.fillAmount = value;
    }
}
