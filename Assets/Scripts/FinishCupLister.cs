using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCupLister : MonoBehaviour
{
    [HideInInspector] public List<Cup> cups = new List<Cup>();
    [HideInInspector] public Character character;

    public void PutToList(Cup t)
    {
        if (cups.Contains(t)) return;
        t.StopAllCoroutines();
        cups.Add(t);
        Redraw();
        Global.data.Vibrate();
    }

    private void Redraw()
    {
        for (int i = 0; i < cups.Count; i++)
        {
            cups[i].SetTarget(transform.GetChild(i % 8).position);
        }
    }

}
