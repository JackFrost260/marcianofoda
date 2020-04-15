using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CLSurroundTextures : CrosshairLayerBaseRectangles
{
    [Space(10)]
    public Sprite Texture;
    public bool UseOriginalTextureSize = true;
    public Vector2 Size = new Vector2(10, 10);

    [Header("<Surround Arrangement>")]
    public int NTextures = 4;
    public float CenterGap = 30;
    public bool StepAngleForceEqualDivision = true;
    public float StepAngle = 90;

    [Header("<Recoil Response>")]
    public bool EnableRecoilResponse = false;
    public float RecoilToCenterGapPercentage = 100;
    public Vector2 RecoilToSizePercentage;

    [Header("<Transform>")]
    public Vector2 ElementAlignment = new Vector2(0.5f, 0.5f);
    public Vector2 ElementScale = new Vector2(1f, 1f);
    public Vector2 ElementRotationPivot = new Vector2(0.5f, 0.5f);
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
        NTextures = Mathf.Max(1, NTextures);
        Size.x = Mathf.Max(0, Size.x);
        Size.y = Mathf.Max(0, Size.y);
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
            Size,                                   // size
            ElementAlignment,                       // elementAlignment
            ElementRotationPivot,                   // elementRotationPivot
            RecoilToSizePercentage,                 // recoilResponsePercentageSize
            Color.black);                           // outsideColor

        UpdateBaseVariables(
            NTextures,                              // totalElements
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
        Size = size;
    }
}
