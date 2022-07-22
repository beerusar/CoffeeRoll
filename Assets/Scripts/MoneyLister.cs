using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Structs
[System.Serializable]
public struct Seperator
{
    public string title;
    public int money;
    public int cup;
    public Color color;
} 
#endregion

public class MoneyLister : MonoBehaviour
{
    public GameObject prefab;
    public float diff = 5;
    public Seperator[] seperators;

    private void Start()
    {
        var s = 0;
        for (int i = 1; i <= 100; i++)
        {
            var money = (int)(i * diff);
            if (money == seperators[s].money)
            {
                Instantiate(prefab, transform).GetComponent<MoneyListElement>().Draw(seperators[s].cup, money, seperators[s].color, seperators[s].title);
                s++;
                i++;
                if (s >= seperators.Length)
                {
                    break;
                }
            }
            else
            {
                Color.RGBToHSV(seperators[s].color, out float hue, out float sat, out float bri);
                var diff = i % 2 == 0 ? 0.06f : 0.12f;
                var c = Color.HSVToRGB(hue, Mathf.Clamp01(sat - 0.10f), Mathf.Clamp01(bri - diff));
                Instantiate(prefab, transform).GetComponent<MoneyListElement>().Draw(seperators[s].cup, money, c, "");
            }
        }
    }
}
