using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairLayerBaseRectangles : CrosshairLayerBase
{
    // hidden
    protected Sprite texture;
    protected bool useOriginalTextureSize;
    protected Vector2 size = new Vector2(4, 80);
    protected Vector2 elementAlignment = new Vector2(0.5f, 0.5f);
    protected Vector2 elementRotationPivot = new Vector2(0.5f, 0.5f);
    protected Vector2 recoilResponsePercentageSize;
    protected Color outsideColor = new Color(0f, 0f, 0f, 1f);

    // privates
    private Vector2 effectiveSize;
    private Color effectiveOutsideColor;
    private int spriteInstanceID = 0;

    override public void CreateCrosshair()
    {
        base.CreateObject("CrosshairRectangles");

        if (texture != null)
            spriteInstanceID = texture.GetInstanceID();
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

        if (texture != null && texture.GetInstanceID() != spriteInstanceID)
            return true;

        if (texture == null && spriteInstanceID != 0)
        {
            spriteInstanceID = 0;
            return true;
        }

        return false;
    }

    override protected void UpdateCrosshair()
    {
        base.UpdateCrosshair();
    }

    override protected void PreRender()
    {
        base.PreRender();

        if (useOriginalTextureSize && texture != null && texture.texture != null)
        {
            size.x = texture.texture.width;
            size.y = texture.texture.height;
        }

        effectiveSize = (Vector2.Scale(size, elementScale) + recoilResponsePercentageSize * 0.01f * GetWeaponRecoilResponse()) * crosshairScale;

        if (fillOutside)
        {
            materialImageSize.x = Screen.width;
            materialImageSize.y = Screen.height;
            effectiveOutsideColor = outsideColor;
        }
        else
        {
            var range = CrosshairLibs.Abs(elementAlignment - elementRotationPivot) + CrosshairLibs.Abs(elementRotationPivot - new Vector2(0.5f, 0.5f)) + new Vector2(0.5f, 0.5f);
            var hypot = CrosshairLibs.Hypotenuse(effectiveSize);
            var drawRange = range * hypot;
            float sizeOfMaterial = (Mathf.Abs(effectiveGap) + effectiveAAFilterSize + Mathf.Max(drawRange.x, drawRange.y) + 2) * 2f;
            materialImageSize.x = sizeOfMaterial;
            materialImageSize.y = sizeOfMaterial;
            effectiveOutsideColor = new Color(0f, 0f, 0f, 0f);
        }
    }

    override protected void Render()
    {
        base.Render();

        material.SetVector("size", effectiveSize);
        material.SetVector("sizePivot", elementAlignment);
        material.SetVector("rotationPivot", elementRotationPivot);
        material.SetColor("bgColor", effectiveOutsideColor);
        material.SetColor("layerColor", effectiveColorAndOpacity);

        if (texture != null && texture.texture != null)
        {
            material.SetFloat("AAFilterSize", 0);
            material.SetTexture("tex", texture.texture);
        }

        if (fillOutside)
        {
            material.SetVector("translation", effectiveTranslation);
        }
    }

    protected void UpdateRectangleVariables(
        Sprite texture,
        bool useOriginalTextureSize,
        Vector2 size,
        Vector2 elementAlignment,
        Vector2 elementRotationPivot,
        Vector2 recoilResponsePercentageSize,
        Color outsideColor)
    {
        this.texture = texture;
        this.useOriginalTextureSize = useOriginalTextureSize;
        this.size = size;
        this.elementAlignment = elementAlignment;
        this.elementRotationPivot = elementRotationPivot;
        this.recoilResponsePercentageSize = recoilResponsePercentageSize;
        this.outsideColor = outsideColor;
    }
}
