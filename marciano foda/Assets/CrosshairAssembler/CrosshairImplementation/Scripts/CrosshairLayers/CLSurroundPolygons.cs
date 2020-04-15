using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLSurroundPolygons : CrosshairLayerBasePolygons
{
    [Space(10)]
    public int PolygonSides = 3;
    public float PolygonRadius = 5;
    public float LineThickness = 1;
    public bool Fill = false;

    [Header("<Surround Arrangement>")]
    public int NPolygons = 4;
    public float CenterGap = 50;
    public bool StepAngleForceEqualDivision = true;
    public float StepAngle = 90;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCenterGapPercentage = 100;
    public float RecoilToCircleRadiusPercentage = 0;

    [Header("<Transform>")]
    public Vector2 ElementScale = new Vector2(1, 1);
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
        PolygonSides = Mathf.Clamp(PolygonSides, 3, 128);
        PolygonRadius = Mathf.Max(0, PolygonRadius);
        LineThickness = Mathf.Max(0, LineThickness);
        NPolygons = Mathf.Max(1, NPolygons);
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
        PostUpdateVariables();
    }

    private void UpdateVariables()
    {
        UpdatePolygonVariables(
            PolygonSides,                           // polygonSides
            PolygonRadius,                          // radius
            LineThickness,                          // thickness
            Fill,                                   // fill
            RecoilToCircleRadiusPercentage);        // recoilResponsePercentageRadius

        UpdateBaseVariables(
            NPolygons,                              // totalElements
            CenterGap,                              // gap
            StepAngleForceEqualDivision,            // forceEqualDivision
            StepAngle,                              // stepAngle
            EnableRecoilResponse,                   // enableRecoilResponse
            RecoilToCenterGapPercentage,            // recoilResponsePercentageCenterGap
            ElementScale,                           // elementScale
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
