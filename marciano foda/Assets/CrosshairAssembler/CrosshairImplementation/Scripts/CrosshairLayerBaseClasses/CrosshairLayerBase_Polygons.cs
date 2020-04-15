using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairLayerBasePolygons : CrosshairLayerBase
{
    // hidden
    protected int polygonSides = 3;
    protected float radius = 40;
    protected float thickness = 4;
    protected bool fill = false;
    protected float recoilResponsePercentageRadius;

    // privates
    private float effectiveRadius;
    private float effectiveThickness;
    private float rotateToLaidPolygon;

    override public void CreateCrosshair()
    {
        base.CreateObject("CrosshairPolygons");
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
        float sizeOfMaterial = (effectiveRadius * Mathf.Max(elementScale.x, elementScale.y) + Mathf.Abs(effectiveGap) + thickness + effectiveAAFilterSize + 2) * 2;
        materialImageSize.x = sizeOfMaterial;
        materialImageSize.y = sizeOfMaterial;

        rotateToLaidPolygon = 180f / polygonSides * ((polygonSides + 1) % 2);
    }

    override protected void Render()
    {
        base.Render();

        material.SetFloat("nGonFloat", polygonSides);
        material.SetFloat("radius", effectiveRadius);
        material.SetFloat("thickness", effectiveThickness);
        material.SetFloat("fillInside", fill ? 1f : 0f);
        material.SetFloat("rotateToLaidPolygon", rotateToLaidPolygon);
    }

    protected void UpdatePolygonVariables(
        int polygonSides,
        float radius,
        float thickness,
        bool fill,
        float recoilResponsePercentageRadius)
    {
        this.polygonSides = polygonSides;
        this.radius = radius;
        this.thickness = thickness;
        this.fill = fill;
        this.recoilResponsePercentageRadius = recoilResponsePercentageRadius;
    }
}
