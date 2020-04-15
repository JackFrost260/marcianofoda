using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLScope : CrosshairLayerBaseRectangles
{
    [Space(10)]
    public Sprite CenterTexture;
    public bool FillOutside = true;
    public Color OutsideColor = Color.black;

    [Header("<Transform>")]
    public Vector2 ElementScale = new Vector2(1f, 1f);
    public float ElementAngle;
    public Vector2 LayerTranslation;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;


    void OnValidate()
    {
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
    }

    private void UpdateVariables()
    {
        UpdateRectangleVariables(
            CenterTexture,                          // texture
            true,                                   // useOriginalTextureSize
            Vector2.zero,                           // size
            new Vector2(0.5f, 0.5f),                // elementAlignment
            new Vector2(0.5f, 0.5f),                // elementRotationPivot
            Vector2.zero,                           // recoilResponsePercentageSize
            OutsideColor);                          // outsideColor

        UpdateBaseVariables(
            1,                                      // totalElements
            gap,                                    // gap
            false,                                  // forceEqualDivision
            0,                                      // stepAngle
            false,                                  // enableRecoilResponse
            0,                                      // recoilResponsePercentageCenterGap
            ElementScale,                           // elementScale
            ElementAngle,                           // elementAngle
            0,                                      // layerAngle
            LayerTranslation,                       // layerTranslation
            0,                                      // autoRotationRpmElement
            0,                                      // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            FillOutside);                           // fillOutside
    }


}
