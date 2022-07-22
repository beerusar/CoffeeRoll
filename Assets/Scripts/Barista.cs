using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Barista : MonoBehaviour
{
    #region Variables
    public CupLister take;
    public CupLister put;
    public Transform hand;
    public GameObject panel;

    [HideInInspector] public bool takeMode;
    [HideInInspector] public int taken;
    [HideInInspector] public List<TableCup> cups = new List<TableCup>();

    private NavMeshAgent agent;
    private float timer;
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
    public void UpgradeSpeed()
    {
        Global.data.variables.money -= Global.data.variables.baristaSpeedPrice;
        Global.data.variables.baristaSpeedPrice += 50;
        Global.data.variables.baristaSpeed += 0.5f;
        agent.speed = Global.data.variables.baristaSpeed;
        UpdatePanelText();
        Global.data.Save();
    }

    public void UpgradeCapacity()
    {
        Global.data.variables.money -= Global.data.variables.baristaCapacityPrice;
        Global.data.variables.baristaCapacityPrice += 50;
        Global.data.variables.baristaCapacity += 1;
        UpdatePanelText();
        Global.data.Save();
    }

    public void UpdatePanelText()
    {
        var capacity = panel.transform.Find("Content/Capacity").GetComponentInChildren<Button>();
        if (Global.data.variables.baristaCapacity <= 6)
        {
            capacity.interactable = Global.data.variables.money >= Global.data.variables.baristaCapacityPrice;
            capacity.GetComponentInChildren<Text>().text = "$ " + Global.data.variables.baristaCapacityPrice.ToString();
        }
        else
        {
            capacity.interactable = false;
            capacity.GetComponentInChildren<Text>().text = "Full";
        }

        var speed = panel.transform.Find("Content/Speed").GetComponentInChildren<Button>(); ;
        if (Global.data.variables.baristaSpeed <= 4.5f)
        {
            speed.interactable = Global.data.variables.money >= Global.data.variables.baristaSpeedPrice;
            speed.GetComponentInChildren<Text>().text = "$ " + Global.data.variables.baristaSpeedPrice.ToString();
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
        panel.SetActive(true);

        UpdatePanelText();

        StopAllCoroutines();
        StartCoroutine(PanelAnim(Vector3.one));
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        StartCoroutine(PanelAnim(Vector3.zero, false, 12f));
    }

    public void GoToTake()
    {
        timer = 1;
        agent.SetDestination(take.GetAgentPos());
        takeMode = true;
    }

    public void GoToPut()
    {
        timer = 1;
        agent.SetDestination(put.GetAgentPos());
        takeMode = false;
    }

    public void TakeCup()
    {
        var cup = take.TakeFromList();
        if (cup)
        {
            cups.Add(cup);
            cup.transform.parent = hand;
            var height = 0.4f * (cups.Count - 1);
            cup.SetTarget(Vector3.up * height, 8, true);
            taken++;
        }
    }

    public void PutCup()
    {
        var i = cups.Count - 1;
        var cup = cups[i];
        cups.RemoveAt(i);
        cup.transform.parent = null;
        put.PutToList(cup);
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        if (!hand) hand = transform.Find("Hand");
        agent = GetComponent<NavMeshAgent>();
        GoToTake();
        agent.speed = Global.data.variables.baristaSpeed;
    }

    private void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    timer += Time.deltaTime;
                    if (timer >= 2.5f / Global.data.variables.baristaSpeed)
                    {
                        timer = 0;
                        if (takeMode)
                        {
                            if (cups.Count < Global.data.variables.baristaCapacity && taken < Global.data.cups)
                            {
                                TakeCup();
                            }
                            else if (cups.Count > 0)
                            {
                                GoToPut();
                            }
                        }
                        else
                        {
                            if (cups.Count > 0)
                            {
                                PutCup();
                            }
                            else
                            {
                                GoToTake();
                            }
                        }
                    }
                }
            }
        }
    } 
    #endregion
}
