using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelElement : MonoBehaviour
{
    public Text title, btnText;
    public Button button;

    public void Draw(string t, int level, bool recipe, bool selected = false)
    {
        if (!selected)
        {
            if (level <= Global.data.variables.level) // unlocked
            {
                title.text = t;
                btnText.text = "Select";
                button.interactable = true;
                button.GetComponent<Image>().color = Global.data.unselectedColor;
                if (recipe)
                {
                    button.onClick.AddListener(() =>
                    {
                        Global.data.variables.currentRecipe = transform.GetSiblingIndex();
                        GetComponentInParent<SkinRecipePanel>().Redraw();
                        Global.data.Save();
                    });
                }
                else
                {
                    button.onClick.AddListener(() =>
                    {
                        Global.data.variables.currentSkin = transform.GetSiblingIndex();
                        GetComponentInParent<SkinRecipePanel>().Redraw();
                        Global.data.Save();
                    });
                }

            }
            else
            {
                title.text = "?";
                btnText.text = "Level " + level.ToString();
                button.interactable = false;
                button.GetComponent<Image>().color = Global.data.unselectedColor;
            }            
        }
        else
        {
            title.text = t;
            btnText.text = "Selected";
            button.interactable = true;
            button.GetComponent<Image>().color = Global.data.selectedColor;
        }
    }
}
