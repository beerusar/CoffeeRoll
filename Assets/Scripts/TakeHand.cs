using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeHand : MonoBehaviour
{
    #region Variables
    public int money;
    public bool started;

    private Character character;
    private Collider col; 
    #endregion

    #region Coroutines
    private IEnumerator GoLerp()
    {
        var target = transform.position + transform.right * 7f;
        while (true)
        {
            if (transform.localScale.sqrMagnitude < 0.01f)
            {
                transform.position = target;
                Destroy(transform.gameObject);
                break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);
            yield return null;
        }
    }
    #endregion

    #region Functions
    public void TakeHandDisable()
    {
        started = true;
        StartCoroutine(GoLerp());
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        col = GetComponent<Collider>();
        character = GameObject.FindGameObjectWithTag("Character").GetComponent<Character>();
    }

    private void Update()
    {
        if (!started)
        {
            if (transform.position.z + 3 < character.transform.position.z)
            {
                TakeHandDisable();
            }
            else
            {
                var target = transform.position;
                var x = character.hand.transform.position.x;
                target.x = (transform.right.x > 0) ? Mathf.Clamp(x, 0, 3.25f) : Mathf.Clamp(x, -3.25f, 0);
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2f);
            }
        }
    } 
    #endregion
}
