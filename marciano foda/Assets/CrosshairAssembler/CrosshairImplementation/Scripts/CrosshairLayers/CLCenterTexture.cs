using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLCenterTexture : CrosshairLayerBaseRectangles
{
    [Space(10)]
    public Sprite Texture;
    public bool UseOriginalTextureSize = true;
    public Vector2 LineSize = new Vector2(10, 10);

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public Vector2 RecoilToSizePercentage;

    [Header("<Transform>")]
    public Vector2 ElementAlignment = new Vector2(0.5f, 0.5f);
    public Vector2 ElementScale = new Vector2(1f, 1f);
    public float ElementAngle;
    public Vector2 LayerTranslation;

    [Header("<Animation - Auto Rotation>")]
    public float AutoRotationRpmElement;

    [Header("<Color>")]
    public Color ColorAndOpacity = new Color(1f, 1f, 1f, 1f);
    public bool IgnoreGlobalColor;


    void OnValidate()
    {
        LineSize.x = Mathf.Max(0, LineSize.x);
        LineSize.y = Mathf.Max(0, LineSize.y);
    }

    void Update()
    {
        UpdateVariables();
        UpdateCrosshair();
        PostUpdateVariables();
    }

    private void UpdateVariables()
    {
        UpdateRectangleVariables(
            Texture,                                // texture
            UseOriginalTextureSize,                 // useOriginalTextureSize
            LineSize,                               // size
            ElementAlignment,                       // elementAlignment
            new Vector2(0.5f, 0.5f),                // elementRotationPivot
            RecoilToSizePercentage,                 // recoilResponsePercentageSize
            Color.black);                           // outsideColor

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
            AutoRotationRpmElement,                 // autoRotationRpmElement
            0,                                      // autoRotationRpmLayer
            ColorAndOpacity,                        // colorAndOpacity
            IgnoreGlobalColor,                      // ignoreGlobalColor
            false);                                 // fillOutside
    }

    private void PostUpdateVariables()
    {
        LineSize = size;
    }
}
