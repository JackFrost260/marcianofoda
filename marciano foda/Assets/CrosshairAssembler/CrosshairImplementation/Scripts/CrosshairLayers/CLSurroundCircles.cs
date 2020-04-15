using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLSurroundCircles : CrosshairLayerBaseCircles
{
    [Space(10)]
    public float CircleRadius = 10;
    public float LineThickness = 2;
    public bool Fill = false;

    [Header("<Surround Arrangement>")]
    public int NCircles = 3;
    public float CenterGap = 50;
    public bool StepAngleForceEqualDivision = true;
    public float StepAngle = 120;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCenterGapPercentage = 100;
    public float RecoilToCircleRadiusPercentage = 0;

    [Header("<Transform>")]
    public float LayerAngle;
    public Vector2 LayerTranslation;

    [Header("<Animation - Auto Rotation>")]
    public float AutoRotationRPMLayer;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;

    void OnValidate()
    {
        CircleRadius = Mathf.Max(0, CircleRadius);
        LineThickness = Mathf.Max(0, LineThickness);
        NCircles = Mathf.Max(1, NCircles);
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
            360,                                    // arcAngle
            LineThickness,                          // thickness
            Fill,                                   // fill
            RecoilToCircleRadiusPercentage);        // recoilResponsePercentageRadius

        UpdateBaseVariables(
            NCircles,                               // totalElements
            CenterGap,                              // gap
            StepAngleForceEqualDivision,            // forceEqualDivision
            StepAngle,                              // stepAngle
            EnableRecoilResponse,                   // enableRecoilResponse
            RecoilToCenterGapPercentage,            // recoilResponsePercentageCenterGap
            new Vector2(1, 1),                      // elementScale
            0,                                      // elementAngle
            LayerAngle,                             // layerAngle
            LayerTranslation,                       // layerTranslation
            0,                                      // autoRotationRpmElement
            AutoRotationRPMLayer,                   // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            false);                                 // fillOutside
    }

    private void PostUpdateVariables()
    {
        StepAngle = stepAngle;
    }
}
