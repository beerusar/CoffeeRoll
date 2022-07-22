using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#region Structs
[System.Serializable]
public struct Recipe
{
    public string name;
    public int level;
    public GameObject[] prefabs;
}

[System.Serializable]
public struct Skin
{
    public string name;
    public int level;
    public Material material;
}
#endregion

#region Classes
[System.Serializable]
public class Data
{

    [Header("Waiter")]
    public int waiterCapacity = 2;
    public float waiterSpeed = 2.5f;
    public int waiterCapacityPrice, waiterSpeedPrice = 20;

    [Header("Barista")]
    public int baristaCapacity = 2;
    public float baristaSpeed = 2.5f;
    public int baristaCapacityPrice, baristaSpeedPrice = 20;

    [Header("Progress")]
    public int startCupCount = 1;
    public int money, level;
    public int currentSkin, currentRecipe = 0;
    public int decorationTable, decorationFlower = 0;
    public List<int> boughtTables = new List<int>();

    [HideInInspector] public bool baristaBought;
    [HideInInspector] public bool endMessageStatus;
}

[System.Serializable]
public class Settings
{
    public bool sfx;
    public bool vibration;

} 
#endregion

public class Global : MonoBehaviour
{
    public static Global data;

    #region Variables
    [Header("Debug Settings")]
    public bool dontLoad;
    public bool dontSave;

    [Header("Constants")]
    public int cupPrice = 5;
    public int maxLevel;
    public float characterSpeed = 6f;
    public GameObject cupPrefab;

    [Header("Enhancements")]
    public Skin[] skins;
    public Recipe[] recipes;

    [Header("Color Palette")]
    public Color green;
    public Color red;
    public Color selectedColor, unselectedColor;

    [Header("Game Data")]
    public Data variables;
    public Settings settings;

    [HideInInspector] public int cups; 
    #endregion

    #region Functions
    public void Save()
    {
        if (!dontSave)
        {
            string path = Path.Combine(Application.persistentDataPath, "save.json");
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(JsonUtility.ToJson(variables));
            writer.Close();
        }
    }

    public void SaveSettings()
    {
        if (!dontSave)
        {
            string path = Path.Combine(Application.persistentDataPath, "settings.json");
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(JsonUtility.ToJson(settings));
            writer.Close();
        }
    }

    public void Load()
    {
        if (!dontLoad)
        {
            string path = Path.Combine(Application.persistentDataPath, "save.json");
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                var json = reader.ReadToEnd();
                reader.Close();
                JsonUtility.FromJsonOverwrite(json, variables);
            }

            path = Path.Combine(Application.persistentDataPath, "settings.json");
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                var json = reader.ReadToEnd();
                reader.Close();
                JsonUtility.FromJsonOverwrite(json, settings);
            }
        }
    }

    public void Vibrate(int milliseconds = 50, int power = -1)
    {
        if (settings.vibration)
        {
            RDG.Vibration.Vibrate(milliseconds, power);
        }
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void GoToCafe()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToDecoration()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToLevel()
    {
        variables.level = (int)Mathf.Clamp(variables.level, 1f, maxLevel);
        SceneManager.LoadScene(variables.level + 3);
    }

    public void GoToEnd()
    {
        if (variables.endMessageStatus)
        {
            GoToLevel();
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }
    #endregion

    #region Default Functions
    public void Awake()
    {
        Load();

        if (data == null)
        {
            DontDestroyOnLoad(gameObject);
            data = this;
            if (SceneManager.GetActiveScene().buildIndex == 0)
                GoToLevel();
        }
        else if (data != this)
            Destroy(gameObject);
    } 
    #endregion

}
