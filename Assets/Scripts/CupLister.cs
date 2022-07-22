using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupLister : MonoBehaviour
{

    #region Variables
    [SerializeField] private Vector3 agentPos;

    [HideInInspector] public List<TableCup> cups = new List<TableCup>();
    #endregion

    #region Functions
    public Vector3 GetAgentPos()
    {
        return transform.TransformPoint(agentPos);
    }

    public void PutToList(TableCup t)
    {
        if (cups.Contains(t)) return;
        t.StopAllCoroutines();
        cups.Add(t);
        Redraw();
    }

    public TableCup TakeFromList()
    {
        if (cups.Count == 0) return null;
        var i = cups.Count - 1;
        var cup = cups[i];
        cup.StopAllCoroutines();
        cups.RemoveAt(i);
        Redraw();
        return cup;
    }

    private void Redraw()
    {
        for (int i = 0; i < cups.Count; i++)
        {
            var height = (int)(i / 4) * 0.4f;
            if (i % 4 == 0)
            {
                cups[i].SetTarget(transform.position + new Vector3(0.25f, height, 0.25f));
            }
            else if (i % 4 == 1)
            {
                cups[i].SetTarget(transform.position + new Vector3(-0.25f, height, 0.25f));
            }
            else if (i % 4 == 2)
            {
                cups[i].SetTarget(transform.position + new Vector3(0.25f, height, -0.25f));
            }
            else if (i % 4 == 3)
            {
                cups[i].SetTarget(transform.position + new Vector3(-0.25f, height, -0.25f));
            }
        }
    } 
    #endregion
}
