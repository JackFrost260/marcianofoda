using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairLayerBaseCircles : CrosshairLayerBase
{
    // hidden
    protected float radius = 40;
    protected float arcAngle = 360;
    protected float thickness = 4;
    protected bool fill = false;
    protected float recoilResponsePercentageRadius;

    // privates
    private float effectiveRadius;
    private float effectiveThickness;

    override public void CreateCrosshair()
    {
        base.CreateObject("CrosshairCircles");
    }

    override public void ResetCrosshair()
    {
        DestroyCrosshair();
        CreateCrosshair();
        UpdateCrosshair();
    }

    override public bool IsNeedReset()
    {
        if (childObject == null)
            return true;

        return false;
    }

    override protected void UpdateCrosshair()
    {
        base.UpdateCrosshair();
    }

    override protected void PreRender()
    {
        base.PreRender();

        // radius
        effectiveRadius = (recoilResponsePercentageRadius * 0.01f * GetWeaponRecoilResponse() + radius) * crosshairScale;
        effectiveThickness = thickness * crosshairScale;

        // size
        float sizeOfMaterial = (Mathf.Abs(effectiveRadius) + Mathf.Abs(effectiveGap) + effectiveThickness + effectiveAAFilterSize + 2) * 2;
        materialImageSize.x = sizeOfMaterial;
        materialImageSize.y = sizeOfMaterial;
    }

    override protected void Render()
    {
        base.Render();

        material.SetFloat("arcAngle", arcAngle);
        material.SetFloat("radius", effectiveRadius);
        material.SetFloat("thickness", effectiveThickness);
        material.SetFloat("fill", fill ? 1f : 0f);
    }

    protected void UpdateCircleVariables(
        float radius,
        float arcAngle,
        float thickness,
        bool fill,
        float recoilResponsePercentageRadius)
    {
        this.radius = radius;
        this.arcAngle = arcAngle;
        this.thickness = thickness;
        this.fill = fill;
        this.recoilResponsePercentageRadius = recoilResponsePercentageRadius;
    }
}
