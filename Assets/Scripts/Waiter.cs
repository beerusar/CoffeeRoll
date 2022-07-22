using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waiter : MonoBehaviour
{
    #region Variables
    [Header("UI")]
    public Image levelButton;
    public Text moneyText, cupText;

    [Header("Constants")]
    public Camera cam;
    public Transform hand;
    public CupLister take;
    public GameObject panel;
    public GameObject barisaPlatform;
    public GameObject baristaBuy;

    [HideInInspector] public int given;
    [HideInInspector] public List<TableCup> cups = new List<TableCup>();

    private CharacterController controller;
    private FillTrigger trigger;
    private Table giveTable;
    private Vector3 camOffset;
    private float timer = 0;
    #endregion

    #region Coroutines
    private IEnumerator PanelAnim(Vector3 scale, bool result = true, float speed = 6f)
    {
        while (true)
        {
            if (Vector3.Distance(panel.transform.localScale, scale) < 0.02f)
            {
                panel.transform.localScale = scale;
                panel.SetActive(result);
                break;
            }
            panel.transform.localScale = Vector3.Lerp(panel.transform.localScale, scale, Time.deltaTime * speed);
            yield return null;
        }
    } 
    #endregion

    #region Functions
    public void BuyBarista(int price)
    {
        barisaPlatform.SetActive(true);
        baristaBuy.SetActive(false);
        Global.data.variables.baristaBought = true;
        Global.data.variables.money -= price;
        Global.data.Save();
    }

    public void UpgradeSpeed()
    {
        Global.data.variables.money -= Global.data.variables.waiterSpeedPrice;
        Global.data.variables.waiterSpeedPrice += 50;
        Global.data.variables.waiterSpeed += 0.8f;
        UpdatePanelText();
        Global.data.Save();
    }

    public void UpgradeCapacity()
    {
        Global.data.variables.money -= Global.data.variables.waiterCapacityPrice;
        Global.data.variables.waiterCapacityPrice += 50;
        Global.data.variables.waiterCapacity += 1;
        UpdatePanelText();
        Global.data.Save();
    }

    public void UpdatePanelText()
    {
        var capacity = panel.transform.Find("Content/Capacity").GetComponentInChildren<Button>();
        if (Global.data.variables.waiterCapacity <= 6)
        {
            capacity.interactable = Global.data.variables.money >= Global.data.variables.waiterCapacityPrice;
            capacity.GetComponentInChildren<Text>().text = "$ " + Global.data.variables.waiterCapacityPrice.ToString();
        }
        else
        {
            capacity.interactable = false;
            capacity.GetComponentInChildren<Text>().text = "Full";
        }

        var speed = panel.transform.Find("Content/Speed").GetComponentInChildren<Button>(); ;
        if (Global.data.variables.waiterSpeed <= 6f)
        {
            speed.interactable = Global.data.variables.money >= Global.data.variables.waiterSpeedPrice;
            speed.GetComponentInChildren<Text>().text = "$ " + Global.data.variables.waiterSpeedPrice.ToString();
        }
        else
        {
            speed.interactable = false;
            speed.GetComponentInChildren<Text>().text = "Full";
        }
    }

    public void ShowPanel()
    {
        panel.transform.localScale = Vector3.zero;

        UpdatePanelText();

        panel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(PanelAnim(Vector3.one));
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        StartCoroutine(PanelAnim(Vector3.zero, false, 12f));
    }

    public void Move(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            transform.forward = dir;
            controller.SimpleMove(dir * (Global.data.variables.waiterSpeed + 2.5f));
        }
    }

    public void TakeCup()
    {
        if (cups.Count < Global.data.variables.waiterCapacity)
        {
            var cup = take.TakeFromList();
            if (cup)
            {
                cups.Add(cup);
                cup.transform.parent = hand;
                var height = 0.4f * (cups.Count - 1);
                cup.SetTarget(Vector3.up * height, 8, true);
            }
        }
    }

    public void GiveCup(Table t)
    {
        if (cups.Count > 0 && t.neededCup > t.givenCup)
        {
            var i = cups.Count - 1;
            cups[i].transform.parent = null;
            t.lister.PutToList(cups[i]);
            cups.RemoveAt(i);
            t.givenCup++;
            given++;
        }
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        if (Global.data.variables.baristaBought)
        {
            BuyBarista(0);
        }

        if (Global.data.variables.level < Global.data.maxLevel)
        {
            levelButton.GetComponentInChildren<Text>().text = "Level\n" + Global.data.variables.level;
            levelButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Global.data.GoToLevel();
            });
        }
        else
        {
            levelButton.GetComponentInChildren<Text>().text = "Level 8";
            levelButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Global.data.GoToEnd();
            });
        }
        if (!hand) hand = transform.Find("Hand");
        if (!cam) cam = Camera.main;
        controller = GetComponent<CharacterController>();
        camOffset = transform.position - cam.transform.position;
    }

    private void Update()
    {
        if (Global.data.cups - given <= 0)
        {
            levelButton.color = Color.Lerp(levelButton.color, Global.data.selectedColor, Time.deltaTime * 12f);
        }

        moneyText.text = Global.data.variables.money.ToString();
        cupText.text = (Global.data.cups - given).ToString();

        if (trigger && trigger.enabled && !trigger.kill)
        {
            timer += Time.deltaTime;
            trigger.progress.value = Mathf.Clamp01(timer / trigger.duration);
            if (timer > trigger.duration)
            {
                trigger.trigger.Invoke();
                timer = 0;
                trigger.progress.value = 0;
                trigger.enabled = trigger.retrigger;
            }
        }
        else if (giveTable)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                if (giveTable.moneyObject.activeSelf && giveTable.money > 0)
                {
                    Global.data.variables.money += giveTable.money;
                    giveTable.money = 0;
                    giveTable.moneyObject.SetActive(false);
                    Global.data.Save();
                }
                else
                {
                    GiveCup(giveTable);
                }
                timer = 0;
            }
        }

        var camTarget = transform.position - camOffset;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camTarget, Time.deltaTime * 8f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FillTrigger"))
        {
            var ft = other.GetComponent<FillTrigger>();
            if (ft == trigger)
            {
                ft.enabled = true;
                if (trigger.kill)
                {
                    other.isTrigger = false;
                    timer = 0;
                    trigger.progress.value = 0;
                }
                else
                {
                    timer = 0;
                    trigger.progress.value = 0;
                }
                trigger = null;
            }
        }
        else if (other.CompareTag("Table"))
        {
            timer = 0;
            giveTable = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FillTrigger"))
        {
            timer = 0;
            trigger = other.GetComponent<FillTrigger>();

        }
        else if (other.CompareTag("Table"))
        {
            timer = 0;
            giveTable = other.GetComponentInParent<Table>();
        }
    } 
    #endregion

}
