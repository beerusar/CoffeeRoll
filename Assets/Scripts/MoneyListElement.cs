using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyListElement : MonoBehaviour
{
    public Image cupImage;
    public Text cupsUI, moneyUI, titleUI;

    [HideInInspector] public int cups, money;
    [HideInInspector] public string title;
    [HideInInspector] public Color back;

    public void Draw(int cups, int money, Color back, string title)
    {
        GetComponent<Image>().color = back;
        cupsUI.text = cups.ToString();
        moneyUI.text = money.ToString();
        if (title.Length > 0)
        {
            cupImage.gameObject.SetActive(false);
            var s = GetComponent<RectTransform>();
            s.sizeDelta = new Vector2(s.sizeDelta.x, s.sizeDelta.y * 2);
            titleUI.gameObject.SetActive(true);
            moneyUI.gameObject.SetActive(false);
            cupsUI.gameObject.SetActive(false);
            titleUI.text = title;
        }
    }

}
