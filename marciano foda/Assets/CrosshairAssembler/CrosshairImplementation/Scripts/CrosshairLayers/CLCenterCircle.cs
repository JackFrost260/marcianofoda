using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLCenterCircle : CrosshairLayerBaseCircles
{
    [Space(10)]
    public float CircleRadius = 20;
    public float LineThickness = 2;
    public bool Fill = false;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCircleRadiusPercentage = 100;

    [Header("<Transform>")]
    public Vector2 LayerTranslation;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;

    void OnValidate()
    {
        CircleRadius = Mathf.Max(0, CircleRadius);
        LineThickness = Mathf.Max(0, LineThickness);
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
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
            1,                                      // totalElements
            0,                                      // gap
            false,                                  // forceEqualDivision
            0,                                      // stepAngle
            EnableRecoilResponse,                   // enableRecoilResponse
            0,                                      // recoilResponsePercentageCenterGap
            new Vector2(1, 1),                      // elementScale
            0,                                      // elementAngle
            0,                                      // layerAngle
            LayerTranslation,                       // layerTranslation
            0,                                      // autoRotationRpmElement
            0,                                      // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            false);                                 // fillOutside
    }
}
