using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    #region Variables
    public Vector3 disableVector;
    private Vector3 target; 
    #endregion

    #region Coroutines
    private IEnumerator DisableAnim()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                transform.position = target;
                break;
            }
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 8f);
            yield return null;
        }
    }
    #endregion

    #region Functions
    public void Disable()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        StartCoroutine(DisableAnim());
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        target = transform.position + disableVector;
    } 
    #endregion

}
