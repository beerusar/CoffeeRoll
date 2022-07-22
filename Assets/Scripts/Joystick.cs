using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{

    #region Variables
    [Header("Characters")]
    public Waiter waiter;
    public Barista barista;

    [Header("UI")]
    [SerializeField] private Image dot, area;

    private Vector3 start, end;
    private float max;
    private bool onUi;
    #endregion

    #region Default Functions
    private void Start()
    {
        max = area.GetComponent<RectTransform>().rect.width / 2;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (Global.data.IsPointerOverUIObject())
            {
                onUi = true;
                return;
            }
            if (waiter.panel.activeSelf) waiter.ClosePanel();
            if (barista.panel.activeSelf) barista.ClosePanel();
            start = Input.mousePosition;
            dot.transform.position = Input.mousePosition;
            area.transform.position = Input.mousePosition;
            dot.enabled = true;
            area.enabled = true;
        }
        if (Input.GetMouseButton(0))
        {
            if (!onUi)
            {
                end = Input.mousePosition;
                dot.transform.position = end;
                var diff = transform.InverseTransformVector(end - start);
                if (diff.sqrMagnitude > max * max)
                {
                    area.transform.position += diff - Vector3.ClampMagnitude(diff, max);
                    start = area.transform.position;
                }
                waiter.Move(new Vector3(diff.x, 0, diff.y) / max);
            }
        }
        else
        {
            onUi = false;
            dot.enabled = false;
            area.enabled = false;
        }
    } 
    #endregion

}
