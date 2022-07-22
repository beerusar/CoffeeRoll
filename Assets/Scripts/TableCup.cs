using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCup : MonoBehaviour
{
    #region Variables
    [HideInInspector] public bool listed; 
    #endregion

    #region Coroutines
    private IEnumerator KillAnim()
    {
        while (true)
        {
            if (transform.localScale.sqrMagnitude < 0.0025f)
            {
                Destroy(gameObject);
                break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private IEnumerator GoTo(Vector3 pos, float speed, bool local)
    {
        if (local)
        {
            while (true)
            {
                if (Vector3.Distance(transform.localPosition, pos) < 0.06f)
                {
                    transform.localPosition = pos;
                    break;
                }
                transform.localPosition = Vector3.Lerp(transform.localPosition, pos, Time.deltaTime * speed);
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, pos) < 0.06f)
                {
                    transform.position = pos;
                    break;
                }
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
                yield return null;
            }
        }
    }
    #endregion

    #region Functions
    public void Kill()
    {
        StartCoroutine(KillAnim());
    }

    public void SetTarget(Vector3 pos, float speed = 5f, bool local = false)
    {
        StartCoroutine(GoTo(pos, speed, local));
    } 
    #endregion

    #region Default Functions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CupLister") && !listed)
        {
            listed = true;
            var cupLister = other.GetComponent<CupLister>();
            cupLister.PutToList(this);
        }
    } 
    #endregion
}
