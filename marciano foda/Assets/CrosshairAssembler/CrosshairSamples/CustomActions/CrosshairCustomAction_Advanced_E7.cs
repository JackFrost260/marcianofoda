using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairCustomAction_Advanced_E7 : CrosshairCustomAction
{
    private float addAngle = 0;

    override public void UpdateCustomAction()
    {
        var polygonLayer = GetCrosshairLayer("sidePolygons") as CLSurroundPolygons;
        if (polygonLayer == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            polygonLayer.LayerAngle += addAngle;
            addAngle = 45;
        }

        if (Input.GetMouseButton(0) && addAngle == 0)
        {
            addAngle = 45;
        }

        if (addAngle > 0)
        {
            float add = Mathf.Min(Time.deltaTime * 300, addAngle);
            polygonLayer.LayerAngle += add;
            addAngle -= add;
        }
    }
}
