using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region Structs
[System.Serializable]
public struct DecorationItem
{
    public int price;
    public float showAngle;
    public GameObject item;
}

#endregion

public class Decorator : MonoBehaviour
{
    #region Variables

    public DecorationItem[] tables;
    public DecorationItem[] flowers;

    [Header("UI")]
    public Button backButton;
    public Button tableButton, flowerButton;
    public Text money;
    private Image tableButtonImage, flowerButtonImage;
    private Text tableButtonText, flowerButtonText;
    #endregion

    #region Functions
    public void Draw()
    {
        money.text = Global.data.variables.money.ToString();

        for (int i = 0; i < tables.Length; i++)
        {
            if (i >= Global.data.variables.decorationTable) break;
            tables[i].item.SetActive(true);
        }

        for (int i = 0; i < flowers.Length; i++)
        {
            if (i >= Global.data.variables.decorationFlower) break;
            flowers[i].item.SetActive(true);
        }

        if (Global.data.variables.decorationTable < tables.Length)
        {
            var price = tables[Global.data.variables.decorationTable].price;
            tableButtonText.text = "$ " + price.ToString();
            if (Global.data.variables.money >= price)
            {
                tableButton.interactable = true;
                tableButtonImage.color = Global.data.green;
            }
            else
            {
                tableButton.interactable = false;
                tableButtonImage.color = Global.data.red;
            }
        }
        else
        {
            tableButtonText.text = "Full";
            tableButton.interactable = false;
            tableButtonImage.color = Global.data.red;
        }

        if (Global.data.variables.decorationFlower < flowers.Length)
        {
            var price = flowers[Global.data.variables.decorationFlower].price;
            flowerButtonText.text = "$ " + price.ToString();
            if (Global.data.variables.money >= price)
            {
                flowerButton.interactable = true;
                flowerButtonImage.color = Global.data.green;
            }
            else
            {
                flowerButton.interactable = false;
                flowerButtonImage.color = Global.data.red;
            }
        }
        else
        {
            flowerButtonText.text = "Full";
            flowerButton.interactable = false;
            flowerButtonImage.color = Global.data.red;
        }


    }

    public void BuyTable()
    {
        var price = tables[Global.data.variables.decorationTable].price;
        if (Global.data.variables.money >= price)
        {
            GetComponent<CameraRotator>().LookAt(tables[Global.data.variables.decorationTable].showAngle);
            Global.data.variables.money -= price;
            Global.data.variables.decorationTable++;
            Draw();
            Global.data.Save();
        }
    }

    public void BuyFlower()
    {
        var price = flowers[Global.data.variables.decorationFlower].price;
        if (Global.data.variables.money >= price)
        {
            GetComponent<CameraRotator>().LookAt(flowers[Global.data.variables.decorationFlower].showAngle);
            Global.data.variables.money -= price;
            Global.data.variables.decorationFlower++;
            Draw();
            Global.data.Save();
        }
    } 
    #endregion

    #region Default Functions
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            Global.data.GoToLevel();
        });
        tableButtonImage = tableButton.GetComponent<Image>();
        tableButtonText = tableButton.GetComponentInChildren<Text>();
        flowerButtonImage = flowerButton.GetComponent<Image>();
        flowerButtonText = flowerButton.GetComponentInChildren<Text>();
        Draw();
    } 
    #endregion

}
