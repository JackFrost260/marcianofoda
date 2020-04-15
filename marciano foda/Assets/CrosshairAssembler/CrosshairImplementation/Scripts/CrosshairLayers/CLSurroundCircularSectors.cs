using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLSurroundCircularSectors : CrosshairLayerBaseCircles
{
    [Space(10)]
    public float CircularSectorRadius = 20;
    public float CircularSectorAngle = 60;

    [Header("<Surround Arrangement>")]
    public int NCircularSectors = 3;
    public float CenterGap = 10;
    public bool StepAngleForceEqualDivision = true;
    public float StepAngle = 120;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCenterGapPercentage = 100;
    public float RecoilToCircleRadiusPercentage = 0;

    [Header("<Transform>")]
    public float ElementAngle;
    public float LayerAngle;
    public Vector2 LayerTranslation;

    [Header("<Animation - Auto Rotation>")]
    public float AutoRotationRpmElement;
    public float AutoRotationRpmLayer;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;

    void OnValidate()
    {
        CircularSectorRadius = Mathf.Max(0, CircularSectorRadius);
        NCircularSectors = Mathf.Max(1, NCircularSectors);
        CircularSectorAngle = Mathf.Clamp(CircularSectorAngle, 0, 360);
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
        PostUpdateVariables();
    }

    private void UpdateVariables()
    {
        UpdateCircleVariables(
            CircularSectorRadius,                   // radius
            CircularSectorAngle,                    // arcAngle
            0,                                      // thickness
            true,                                   // fill
            RecoilToCircleRadiusPercentage);        // recoilResponsePercentageRadius

        UpdateBaseVariables(
            NCircularSectors,                       // totalElements
            CenterGap,                              // gap
            StepAngleForceEqualDivision,            // forceEqualDivision
            StepAngle,                              // stepAngle
            EnableRecoilResponse,                   // enableRecoilResponse
            RecoilToCenterGapPercentage,            // recoilResponsePercentageCenterGap
            new Vector2(1, 1),                      // elementScale
            ElementAngle,                           // elementAngle
            LayerAngle,                             // layerAngle
            LayerTranslation,                       // layerTranslation
            AutoRotationRpmElement,                 // autoRotationRpmElement
            AutoRotationRpmLayer,                   // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            false);                                 // fillOutside
    }

    private void PostUpdateVariables()
    {
        StepAngle = stepAngle;
    }
}
