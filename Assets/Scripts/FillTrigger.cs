using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FillTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 3f;
    public bool kill, retrigger;
    public UnityEvent trigger;

    [Header("UI")]
    public ProgressBar progress;
    public Canvas canvas;

    private void Start()
    {
        if (!canvas) canvas = GetComponentInChildren<Canvas>();
        if (!progress) progress = GetComponentInChildren<ProgressBar>();
    }

}
