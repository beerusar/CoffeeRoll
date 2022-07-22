using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{

    private Vector3 start;
    private Vector3 end;
    private bool onUi;
    private float sign;
    private bool hasTarget;
    private float targetAngle;

    public void LookAt(float angle)
    {
        targetAngle = angle;
        hasTarget = true;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (Global.data.IsPointerOverUIObject())
            {
                onUi = true;
                return;
            };
            hasTarget = false;
            start = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (!onUi)
            {
                end = Input.mousePosition;
                var diff = transform.InverseTransformVector(end - start);
                var y = transform.eulerAngles.y - diff.x * Time.deltaTime;
                if (y <= 60 || y >= 300)
                {
                    sign = Mathf.Sign(diff.x);
                    transform.rotation = Quaternion.Euler(0, y, 0);
                }
                else if (sign > 0)
                {
                    start = end;
                    transform.rotation = Quaternion.Euler(0, 300, 0);
                }
                else if (sign < 0)
                {
                    start = end;
                    transform.rotation = Quaternion.Euler(0, 60, 0);
                }
            }
        }
        else
        {
            if (hasTarget)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 3f);
            }
            onUi = false;
        }
    }

}
