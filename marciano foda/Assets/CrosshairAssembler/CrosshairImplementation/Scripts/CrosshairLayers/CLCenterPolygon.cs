using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLCenterPolygon : CrosshairLayerBasePolygons
{
    [Space(10)]
    public int PolygonSides = 3;
    public float PolygonRadius = 20;
    public float LineThickness = 2;
    public bool Fill = false;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCircleRadiusPercentage = 100;

    [Header("<Transform>")]
    public Vector2 ElementScale = new Vector2(1, 1);
    public float ElementAngle;
    public Vector2 LayerTranslation;

    [Header("<Animation - Auto Rotation>")]
    public float AutoRotationRpmLayer;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;

    void OnValidate()
    {
        PolygonSides = Mathf.Clamp(PolygonSides, 3, 128);
        PolygonRadius = Mathf.Max(0, PolygonRadius);
        LineThickness = Mathf.Max(0, LineThickness);
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
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
            1,                                      // totalElements
            0,                                      // gap
            false,                                  // forceEqualDivision
            0,                                      // stepAngle
            EnableRecoilResponse,                   // enableRecoilResponse
            0,                                      // recoilResponsePercentageCenterGap
            ElementScale,                           // elementScale
            ElementAngle,                           // elementAngle
            0,                                      // layerAngle
            LayerTranslation,                       // layerTranslation
            0,                                      // autoRotationRpmElement
            AutoRotationRpmLayer,                   // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            false);                                 // fillOutside
    }
}
