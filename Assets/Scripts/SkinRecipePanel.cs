using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinRecipePanel : MonoBehaviour
{
    #region Variables
    [Header("UI")]
    public Transform skinsPanel;
    public Transform recipesPanel;
    public GameObject panelElement;

    [Header("Other")]
    public Character character;
    #endregion

    #region Functions
    public void Show()
    {
        Redraw();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        foreach (Transform t in character.cupsParent)
        {
            var cup = t.GetComponent<Cup>();
            cup.SetSkin(Global.data.skins[Global.data.variables.currentSkin].material);
        }
        gameObject.SetActive(false);
    }

    public void Redraw()
    {
        for (var i = 0; i < Global.data.recipes.Length; i++)
        {
            var recipe = Global.data.recipes[i];
            recipesPanel.GetChild(i).GetComponent<PanelElement>().Draw(recipe.name, recipe.level, true, Global.data.variables.currentRecipe == i);
        }
        for (var i = 0; i < Global.data.skins.Length; i++)
        {
            var skin = Global.data.skins[i];
            skinsPanel.GetChild(i).GetComponent<PanelElement>().Draw(skin.name, skin.level, false, Global.data.variables.currentSkin == i);
        }
    }
    #endregion

    #region Default Functions
    private void Start()
    {
        for (var i = 0; i < Global.data.recipes.Length; i++)
        {
            var recipe = Global.data.recipes[i];
            Instantiate(panelElement, recipesPanel).GetComponent<PanelElement>().Draw(recipe.name, recipe.level, true, Global.data.variables.currentRecipe == i);
        }
        for (var i = 0; i < Global.data.skins.Length; i++)
        {
            var skin = Global.data.skins[i];
            Instantiate(panelElement, skinsPanel).GetComponent<PanelElement>().Draw(skin.name, skin.level, false, Global.data.variables.currentSkin == i);
        }
        gameObject.SetActive(false);
        transform.localScale = Vector3.one;
    }
    #endregion

}
