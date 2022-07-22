using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    public int index, price;

    [Header("UI")]
    public Text priceText;
    public Image priceBack;
    public Transform coffeeCounter;

    [Header("Childs")]
    public GameObject interactionTrigger;
    public GameObject moneyObject;
    public CupLister lister;

    [Header("Other")]
    public HumanSpawner spawner;

    [HideInInspector] public Vector3 chair1, chair2;
    [HideInInspector] public int neededCup, givenCup;
    [HideInInspector] public int humanCount, arrived;
    [HideInInspector] public int money;

    private bool bought;
    private float pastTime, timer;
    private Text coffeeCounterText;
    private FillTrigger trigger;
    #endregion

    #region Functions
    public void Buy(bool free = false)
    {
        interactionTrigger.SetActive(true);
        GetComponent<MeshRenderer>().enabled = true;
        trigger.canvas.gameObject.SetActive(false);
        if (!free)
        {
            Global.data.variables.money -= price;
            Global.data.variables.boughtTables.Add(index);
        }
        trigger.kill = true;
        bought = true;
        Global.data.Save();
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        if (!moneyObject) moneyObject = transform.Find("Money").gameObject;
        if (!interactionTrigger) interactionTrigger = transform.Find("InteractionTrigger").gameObject;
        if (!lister) lister = transform.Find("Lister").GetComponent<CupLister>();

        coffeeCounterText = coffeeCounter.GetComponentInChildren<Text>();
        var chairDistance = transform.localScale.z / 2 + 0.5f;
        chair1 = transform.position - (transform.forward * chairDistance);
        chair2 = transform.position + (transform.forward * chairDistance);
        trigger = GetComponent<FillTrigger>();
        priceText.text = "$ " + Mathf.FloorToInt(price).ToString();

        if (Global.data.variables.boughtTables.Contains(index))
        {
            Buy(true);
            GetComponent<Collider>().isTrigger = false;
        }
    }

    private void Update()
    {
        if (bought)
        {
            if (money == 0)
            {
                if (humanCount == 0)
                {
                    coffeeCounter.gameObject.SetActive(false);
                    pastTime += Time.deltaTime;
                    if (pastTime > 5)
                    {
                        pastTime = 0;
                        spawner.SpawnAndWalk(this);
                    }
                }
                else if (humanCount == arrived && neededCup == 0)
                {
                    coffeeCounter.gameObject.SetActive(false);
                    neededCup = Mathf.Clamp(Random.Range(1, 5), 1, Global.data.cups);
                }
                else if (humanCount == arrived && neededCup == givenCup)
                {
                    coffeeCounter.gameObject.SetActive(false);
                    timer = 0;
                    money = givenCup * Global.data.cupPrice;
                }
                else if (humanCount == arrived && neededCup > 0)
                {
                    coffeeCounter.position = Camera.main.WorldToScreenPoint(transform.position);
                    coffeeCounter.gameObject.SetActive(true);
                    coffeeCounterText.text = (neededCup - givenCup).ToString();
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > 1f)
                {
                    if (lister.cups.Count > 0)
                    {
                        var i = lister.cups.Count - 1;
                        var cup = lister.cups[i];
                        lister.cups.RemoveAt(i);
                        cup.Kill();
                    }
                    else
                    {
                        givenCup = 0;
                        neededCup = 0;
                        moneyObject.SetActive(true);
                    }
                    timer = 0;
                }
            }        
        }
        else
        {
            if (Global.data.variables.money < price)
            {
                priceBack.color = Global.data.red;
                trigger.enabled = false;
            }
            else
            {
                priceBack.color = Global.data.green;
                trigger.enabled = true;
            }
        }
    }
    #endregion

}
