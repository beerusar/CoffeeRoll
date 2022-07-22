using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Drinks { Empty, Espresso, FrenchPress, Milk, MilkyEspresso, MilkyFrenchPress };

public class Cup : MonoBehaviour
{

    #region Variables
    private Drinks drink = Drinks.Empty;
    private Transform defaultCup, defaultDrinks;
    private Rigidbody rb;
    private Collider col;
    private int recipe = -1;

    [HideInInspector] public Character character;
    [HideInInspector] public bool sleeve, lid;
    [HideInInspector] public int value;
    #endregion

    #region Coroutines
    private IEnumerator GoAway()
    {
        var x = (Random.Range(0f, 1f) > 0.5f ? 1 : -1) * Random.Range(1f, 2f);

        if (Mathf.Abs(transform.position.x + x) > 3.25f)
        {
            x = -x;
        }

        var target = transform.position + new Vector3(x, 0, Random.Range(1.5f, 5f));
        var mid = (transform.position + target) / 2;

        float curve = 0;
        while (true)
        {
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                transform.position = target;
                break;
            }
            var t = Time.deltaTime * 8f;
            curve += t;
            transform.position = Vector3.Slerp(transform.position, target, t);
            yield return null;
        }
    }

    private IEnumerator SellAnim(Vector3 dir)
    {
        var target = transform.position + dir * 7f;
        while (true)
        {
            if (transform.localScale.sqrMagnitude < 0.01f)
            {
                transform.position = target;
                Destroy(gameObject);
                break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime / 1.5f);
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 1.5f);
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
    public void MakeReady()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        col = GetComponent<Collider>();
        defaultCup = transform.Find("Default");
        defaultDrinks = defaultCup.Find("Drinks");
    }

    public void SetSkin(Material skin)
    {
        defaultCup.GetComponent<Renderer>().sharedMaterial = skin;
    }

    public void GoAwayAnim()
    {
        StartCoroutine(GoAway());
    }

    public void SetTarget(Vector3 pos, float speed = 5f, bool local = false)
    {
        StartCoroutine(GoTo(pos, speed, local));
    }

    private void AddValue(int amount)
    {
        value += amount;
        character.AddTotalValue(amount);
    }

    private void Obstacle(bool press = false)
    {
        character.speed = -6;
        Global.data.Vibrate();
        character.PlaySFX(character.obstacle);

        int index = character.cups.IndexOf(transform);
        int count = character.cups.Count - index;

        if (index < 0) return;

        for (int i = 0; i < count; i++)
        {
            Cup cup = character.cups[index].GetComponent<Cup>();
            character.AddTotalValue(-cup.value);
            character.cups.RemoveAt(index);
            if (i == 0 && press)
            {
                transform.localScale = new Vector3(1, 0.02f, 1);
                cup.col.enabled = false;
            }
            else
            {
                cup.GoAwayAnim();
                cup.col.isTrigger = true;
            }
        }
    }

    private void Sell(Transform trigger)
    {
        col.enabled = false;
        character.cups.Remove(transform);
        character.EarnMoney(value);

        character.AddTotalValue(-value);
        Global.data.Vibrate();

        transform.position = new Vector3(transform.position.x, transform.position.y, trigger.position.z);
        var dir = trigger.GetComponentInChildren<ParticleSystem>().transform.forward;
        StartCoroutine(SellAnim(dir));
    }

    private void TakeHand(Collider hand)
    {
        var th = hand.GetComponent<TakeHand>();

        Global.data.Vibrate();

        hand.enabled = false;

        if (th.money > 0)
        {
            character.PlaySFX(character.coin);

            character.AddTotalValue(-value);
            character.EarnMoney(hand.GetComponent<TakeHand>().money + value);
            transform.parent = hand.transform;
            character.cups.Remove(transform);
            transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
            th.TakeHandDisable();
        }
        else
        {
            character.PlaySFX(character.obstacle);

            character.speed = -3;
            int index = character.cups.IndexOf(transform);
            int count = character.cups.Count - index;

            if (index < 0) return;

            for (int i = 0; i < count; i++)
            {
                Cup cup = character.cups[index].GetComponent<Cup>();
                character.AddTotalValue(-cup.value);
                character.cups.RemoveAt(index);
                if (i == 0)
                {
                    cup.col.enabled = false;
                    transform.parent = hand.transform;
                    transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
                    th.TakeHandDisable();
                }
                else
                {
                    cup.GoAwayAnim();
                    cup.col.isTrigger = true;
                }
            }
        }

    }

    private void SetDrink(Drinks drink)
    {
        foreach (Transform t in defaultDrinks)
        {
            t.gameObject.SetActive(t.GetSiblingIndex() == (int)drink - 1);
        }
        AddValue(2);
        this.drink = drink;
        Global.data.Vibrate();
    }

    private void Sleeve()
    {
        if (!sleeve)
        {
            Global.data.Vibrate();
            character.PlaySFX(character.gates);
            defaultCup.Find("Sleeve").gameObject.SetActive(true);
            sleeve = true;
            AddValue(recipe == -1 ? 1 : recipe == 0 ? 2 : 3);
        }
    }

    private void Lid()
    {
        if (!lid)
        {
            Global.data.Vibrate();
            character.PlaySFX(character.gates);
            defaultCup.Find("Lid").gameObject.SetActive(true);
            lid = true;
            AddValue(recipe == -1 ? 1 : recipe == 0 ? 2 : 3);
        }
    }

    private void UpRecipe()
    {
        var prefabs = Global.data.recipes[Global.data.variables.currentRecipe].prefabs;
        if (recipe + 1 < prefabs.Length)
        {
            Global.data.Vibrate();
            character.PlaySFX(character.gates);
            recipe++;
            lid = false;
            sleeve = false;
            Destroy(defaultCup.gameObject);
            defaultCup = Instantiate(prefabs[recipe], transform).transform;
            AddValue(recipe == 0 ? 2 : 3);
        }
    }
    #endregion

    #region Default Functions

    private void OnTriggerEnter(Collider other)
    {
        if (col.isTrigger) return;
        Cup cup = other.gameObject.GetComponent<Cup>();
        if (cup && other.isTrigger)
        {
            other.isTrigger = false;
            cup.StopAllCoroutines();
            character.AddCup(cup);

            Global.data.Vibrate();
            character.PlaySFX(character.cup);
        }
        else if (other.CompareTag("Finish"))
        {
            other.enabled = false;
            character.status = 1;
        }
        else if (other.CompareTag("Finish2"))
        {
            character.cups.Remove(transform);
            character.AddTotalValue(-value);
            character.EarnMoney(value);
            character.finishCupLister.PutToList(this);
        }
        else if (other.CompareTag("Sleeve"))
            Sleeve();
        else if (other.CompareTag("Lid"))
            Lid();
        else if (other.CompareTag("UpCup"))
            UpRecipe();
        else if (other.CompareTag("TakeHand"))
            TakeHand(other);
        else if (other.CompareTag("Obstacle"))
        {
            Obstacle();
            other.GetComponentInParent<Obstacle>().Disable();
        }
        else if (other.CompareTag("DynamicObstacle"))
            Obstacle();
        else if (other.CompareTag("PressObstacle"))
            Obstacle(true);
        else if (!lid && recipe == -1)
        {
            if (drink == Drinks.Empty)
            {
                if (other.CompareTag("Espresso"))
                    SetDrink(Drinks.Espresso);
                else if (other.CompareTag("French Press"))
                    SetDrink(Drinks.FrenchPress);
                else if (other.CompareTag("Milk"))
                    SetDrink(Drinks.Milk);
            }
            else if (drink == Drinks.Milk)
            {
                if (other.CompareTag("Espresso"))
                    SetDrink(Drinks.MilkyEspresso);
                else if (other.CompareTag("French Press"))
                    SetDrink(Drinks.MilkyFrenchPress);
            }
            else if (drink == Drinks.Espresso)
            {
                if (other.CompareTag("Milk"))
                    SetDrink(Drinks.MilkyEspresso);
            }
            else if (drink == Drinks.FrenchPress)
            {
                if (other.CompareTag("Milk"))
                    SetDrink(Drinks.MilkyFrenchPress);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sell"))
            Sell(other.transform);
    }

    #endregion
}
