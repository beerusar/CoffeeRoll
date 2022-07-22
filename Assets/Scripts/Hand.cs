using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    #region Variables
    public Character character;

    [HideInInspector] public bool dismiss;
    #endregion

    #region Default Functions
    private void Start()
    {
        if (character == null) character = GameObject.FindGameObjectWithTag("Character").GetComponent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dismiss) return;

        Cup cup = other.gameObject.GetComponent<Cup>();
        if (cup && other.isTrigger)
        {
            Global.data.Vibrate();
            character.PlaySFX(character.cup);

            other.isTrigger = false;
            cup.StopAllCoroutines();
            character.AddCup(cup);
        }
        else if (other.CompareTag("Finish"))
        {
            other.enabled = false;
            character.status = 1;
        }
        else if (other.CompareTag("Finish3"))
        {
            other.enabled = false;
            character.status = 2;
        }
        else if (other.CompareTag("Obstacle"))
        {
            Global.data.Vibrate();
            character.PlaySFX(character.obstacle);

            character.speed = -6;
            other.GetComponentInParent<Obstacle>().Disable();
        }
        else if (other.CompareTag("DynamicObstacle"))
        {
            Global.data.Vibrate();
            character.PlaySFX(character.obstacle);

            character.speed = -6;
        }
        else if (other.CompareTag("PressObstacle"))
        {
            Global.data.Vibrate();
            character.PlaySFX(character.obstacle);

            character.speed = -6;
        }
    } 
    #endregion

}
