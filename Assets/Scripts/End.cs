using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class End : MonoBehaviour
{
    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            Global.data.variables.startCupCount = 2;
            Global.data.variables.endMessageStatus = true;
            Global.data.variables.endMessageStatus = true;
            Global.data.Save();
            Global.data.GoToLevel();
        });
    }

}
