using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLSurroundArcs : CrosshairLayerBaseCircles
{
    [Space(10)]
    public float CircleRadius = 50;
    public float ArcLengthAngle = 50;
    public float LineThickness = 2;

    [Header("<Surround Arrangement>")]
    public int NArcs = 4;
    public float CenterGap = 0;
    public bool StepAngleForceEqualDivision = true;
    public float StepAngle = 90;

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
        CircleRadius = Mathf.Max(0, CircleRadius);
        LineThickness = Mathf.Max(0, LineThickness);
        NArcs = Mathf.Max(1, NArcs);
        ArcLengthAngle = Mathf.Clamp(ArcLengthAngle, 0, 360);
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
            CircleRadius,                           // radius
            ArcLengthAngle,                         // arcAngle
            LineThickness,                          // thickness
            false,                                  // fill
            RecoilToCircleRadiusPercentage);        // recoilResponsePercentageRadius

        UpdateBaseVariables(
            NArcs,                                  // totalElements
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
