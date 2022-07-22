using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("UI")]
    public RectTransform valuePanel;
    public Button getMoneyButton, cafeButton;
    public GameObject settingsPanel;
    public Toggle vibration, sfx;
    public Text levelText, moneyText, valueText;
    public CanvasGroup uiBeforeStart, scoreboard, endCanvas;

    [Header("Sound Effects")]
    public AudioClip cup;
    public AudioClip coin, gates, obstacle;

    [Header("Other")]
    public FinishCupLister finishCupLister;
    public Hand hand;
    public Camera cam;
    public Transform cupsParent;
    public Vector3 finishCamAngles;

    [HideInInspector] public float speed = 6;
    [HideInInspector] public int earnedMoney, status;
    [HideInInspector] public List<Transform> cups = new List<Transform>();

    private AudioSource audioSource;
    private MoneyLister moneyLister;
    private Transform camTransform;
    private Vector3 camOffset, camTarget, target, scoreboardFront, finishCamOffset;
    private Quaternion normalCamRot, finishCamRot;
    private float start, end, diff, offset, timer;
    private int totalCupsValue, moneyTemp, moneyTargetTemp;
    private bool started, onUi;

    #endregion

    #region Functions
    public void PlaySFX(AudioClip clip)
    {
        if (Global.data.settings.sfx)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void AddCup(Cup cup)
    {
        cups.Add(cup.transform);
        AddTotalValue(cup.value);
    }

    public void AddTotalValue(int amount)
    {
        totalCupsValue += amount;
        valueText.text = totalCupsValue.ToString();
    }
    public void EarnMoney(int amount)
    {
        Global.data.variables.money += amount;
        earnedMoney += amount;
        moneyText.text = Global.data.variables.money.ToString();
    }

    public void OpenSettings()
    {
        vibration.isOn = Global.data.settings.vibration;
        sfx.isOn = Global.data.settings.sfx;
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Global.data.settings.vibration = vibration.isOn;
        Global.data.settings.sfx = sfx.isOn;
        Global.data.SaveSettings();
        settingsPanel.SetActive(false);
    }

    private void Move(Vector3 pos)
    {
        pos.z = offset;
        if (Input.GetMouseButtonDown(0))
        {
            if (Global.data.IsPointerOverUIObject())
            {
                onUi = true;
                return;
            }
            started = true;
            start = Camera.main.ScreenToWorldPoint(pos).x;
            diff = start - transform.position.x;
        }
        if (Input.GetMouseButton(0))
        {
            if (!onUi)
            {
                end = Camera.main.ScreenToWorldPoint(pos).x;
                target.x = Mathf.Clamp(end - diff, -3, 3);
            }
        }
        else
        {
            onUi = false;
        }
    }

    private void CameraHandPoser(float speed = 32f)
    {
        camTarget = new Vector3(camTarget.x / 4, camTarget.y, camTarget.z);
        camTransform.position = Vector3.Lerp(camTransform.position, camTarget, Time.deltaTime * speed);

        var mTarget = Vector3.Lerp(hand.transform.position, transform.position, Time.deltaTime * speed);
        mTarget = new Vector3(mTarget.x, mTarget.y, transform.position.z);
        hand.transform.position = mTarget;

        valuePanel.position = Camera.main.WorldToScreenPoint(mTarget);
    }

    private void CupsPoser()
    {
        if (cups.Count > 0)
        {
            hand.dismiss = true;
            cups[0].position = hand.transform.position;

            for (int i = 1; i < cups.Count; i++)
            {
                var target = transform.position + Vector3.forward * i;
                var x = Mathf.Lerp(cups[i].position.x, target.x, Time.deltaTime * 30f / i);
                cups[i].position = new Vector3(x, 0, target.z);
            }
        }
        else
        {
            hand.dismiss = false;
        }
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        finishCupLister.character = this;

        cafeButton.onClick.AddListener(() => {
            Global.data.GoToDecoration();
        });

        speed = Global.data.characterSpeed;
        audioSource = GetComponent<AudioSource>();
        levelText.text = (SceneManager.GetActiveScene().buildIndex - 3).ToString();
        moneyText.text = Global.data.variables.money.ToString();
        scoreboardFront = scoreboard.transform.position + Vector3.back * 1.5f;
        moneyLister = scoreboard.GetComponentInChildren<MoneyLister>();
        camTransform = cam.transform;

        for (int i = 0; i < Global.data.variables.startCupCount; i++)
        {
            var cup = Instantiate(Global.data.cupPrefab, cupsParent).transform;
            cup.SetAsFirstSibling();
            cups.Add(cup);
            cup.GetComponent<Collider>().isTrigger = false;
        }

        camOffset = transform.position - camTransform.position;
        normalCamRot = camTransform.rotation;
        finishCamRot = Quaternion.Euler(finishCamAngles);
        finishCamOffset = camOffset - new Vector3(0, 3f, -3f);

        offset = Vector3.Distance(transform.position, camTransform.position);

        foreach (Transform t in cupsParent)
        {
            var cup = t.GetComponent<Cup>();
            cup.MakeReady();
            cup.value = 1;
            cup.character = this;
            cup.SetSkin(Global.data.skins[Global.data.variables.currentSkin].material);
        }

        foreach (Transform t in cups)
        {
            var cup = t.GetComponent<Cup>();
            AddTotalValue(cup.value);
        }

    }

    private void Update()
    {
        if (status == 0)
        {
            speed = Mathf.Lerp(speed, Global.data.characterSpeed + (cups.Count == 0 ? 2f : 0f), Time.deltaTime * 2f);
            target = transform.position + (Vector3.forward * Time.deltaTime * speed);
            Move(Input.mousePosition);

            if (started)
            {
                uiBeforeStart.blocksRaycasts = false;
                uiBeforeStart.alpha = uiBeforeStart.alpha > 0.05f ? Mathf.Lerp(uiBeforeStart.alpha, 0, Time.deltaTime * 12f) : 0;
                transform.position = target;
            }

            camTarget = transform.position - camOffset;
            CameraHandPoser();

            CupsPoser();

        }
        else if (status == 1)
        {
            speed = Global.data.characterSpeed + 2f;
            scoreboard.alpha = Mathf.Lerp(scoreboard.alpha, 1, Time.deltaTime * 5f);
            target = new Vector3(0, 0, transform.position.z + Time.deltaTime * speed);
            transform.position = target;

            camTarget = transform.position - finishCamOffset;
            CameraHandPoser(20f);

            camTransform.rotation = Quaternion.Lerp(camTransform.rotation, finishCamRot, Time.deltaTime * 20f);

            CupsPoser();

        }
        else if (status == 2)
        {
            scoreboard.alpha = 1;

            var height = ((float)earnedMoney / moneyLister.diff) * 2f + 1f;

            transform.position = Vector3.Lerp(scoreboardFront, scoreboardFront + Vector3.up * height, Mathf.SmoothStep(0, 1, Mathf.Clamp01(timer)));

            camTarget = transform.position - finishCamOffset;
            CameraHandPoser(2f);

            camTransform.rotation = Quaternion.Lerp(camTransform.rotation, normalCamRot, Time.deltaTime);

            totalCupsValue = Mathf.Clamp((int)(hand.transform.position.y * moneyLister.diff) / 2, 0, earnedMoney);
            valueText.text = totalCupsValue.ToString();

            timer += Time.deltaTime / 2f;

            if (timer > 1.5f && Vector3.Distance(camTarget, camTransform.position) < 0.02f)
            {
                totalCupsValue = earnedMoney;
                valueText.text = totalCupsValue.ToString();
                status++;
            }
        }
        else if (status == 3)
        {
            CameraHandPoser(2f);
            foreach (var s in moneyLister.seperators)
            {
                if (earnedMoney < s.money + moneyLister.diff * 2)
                {
                    Global.data.cups = s.cup;
                    break;
                }
            }
            moneyTemp = Global.data.variables.money;
            moneyTargetTemp = Global.data.variables.money + earnedMoney;
            timer = 0;
            status++;

        }
        else if (status == 4)
        {
            CameraHandPoser(2f);

            timer += Time.deltaTime / 1.5f;

            if (timer > 2f)
            {
                if (Global.data.variables.level < Global.data.maxLevel) Global.data.variables.level++;

                endCanvas.gameObject.SetActive(true);

                if (Global.data.variables.baristaBought || Global.data.variables.money >= 100)
                {
                    getMoneyButton.onClick.AddListener(() =>
                    {
                        Global.data.GoToCafe();
                    });
                }
                else if (Global.data.variables.money >= 20)
                {
                    getMoneyButton.onClick.AddListener(() =>
                    {
                        Global.data.GoToDecoration();
                    });
                }
                else
                {
                    getMoneyButton.onClick.AddListener(() =>
                    {
                        Global.data.GoToLevel();
                    });

                }

                Global.data.Save();

                status++;
            }
        }
        else if (status == 5)
        {
            getMoneyButton.GetComponentInChildren<Text>().text = "Get $ " + earnedMoney.ToString();
            endCanvas.alpha = Mathf.Lerp(endCanvas.alpha, 1, Time.deltaTime * 12f);
        }
    }
    #endregion
}
