using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class CrosshairLayerBase : MonoBehaviour
{
    abstract public bool IsNeedReset();
    abstract public void ResetCrosshair();
    abstract public void CreateCrosshair();

    public string LayerName; // to find a specific layer for Crosshair Custom Action

    // hidden variables that will be assigned from variables of precompiled lyaers
    protected int totalElements;
    protected float gap;
    protected bool forceEqualDivision;
    protected float stepAngle;
    protected bool enableRecoilResponse;
    protected float recoilResponsePercentageCenterGap;
    protected Vector2 elementScale;
    protected float elementAngle;
    protected float layerAngle;
    protected Vector2 layerTranslation;
    protected float autoRotationRpmElement;
    protected float autoRotationRpmLayer;
    protected Color colorAndOpacity;
    protected bool ignoreGlobalColor;
    protected bool fillOutside;

    // common variables
    protected float crosshairScale = 1f;
    protected GameObject childObject; // containing a material(s) to make a shape
    protected Material material;
    protected float elementAutoRotationValue;
    protected float layerAutoRotationValue;
    protected Color effectiveColorAndOpacity;
    protected Vector2 effectiveTranslation;
    protected float effectiveGap;
    protected float effectiveAAFilterSize;
    protected Vector2 materialImageSize;

    void Awake()
    {
        int index = 0;
        while (transform.childCount > index)
        {
            var child = transform.GetChild(index);
            if (!Application.isPlaying && child.name == "Thumbnail")
            {
                ++index;
                continue;
            }
            
            DestroyImmediate(child.gameObject);
        }

        effectiveTranslation = new Vector2();
        materialImageSize = new Vector2();
        elementScale = new Vector2(1, 1);
    }

    protected void CreateObject(string name)
    {
        childObject = new GameObject();
        childObject.transform.parent = this.transform;
        childObject.name = name;
        childObject.transform.localPosition = Vector3.zero;
        childObject.AddComponent<CanvasRenderer>();
        var img = childObject.AddComponent<Image>();
        img.material = material = new Material(Shader.Find("Custom/" + name));
    }

    protected void DestroyCrosshair()
    {
        if (Application.isPlaying)
            Destroy(childObject);
        else
            DestroyImmediate(childObject);
    }

    protected void Idle()
    {
        // update auto rotation
        elementAutoRotationValue += (autoRotationRpmElement / 60f) * 360f * Time.deltaTime;
        layerAutoRotationValue += (autoRotationRpmLayer / 60f) * 360f * Time.deltaTime;
        if (!Application.isPlaying)
        {
            if (autoRotationRpmElement == 0)
                elementAutoRotationValue = 0;
            if (autoRotationRpmLayer == 0)
                layerAutoRotationValue = 0;
        }
    }

    virtual protected void UpdateCrosshair()
    {
        Idle();
        PreRender();
        Render();
    }

    virtual protected void PreRender()
    {
        ComputeRenderingInfo();
    }

    virtual protected void Render()
    {
        OptimizeMaterialSize();
        UpdateMaterialParameters();
        UpdateRectTransform();
    }

    private void ComputeRenderingInfo()
    {
        // scale
        crosshairScale = CrosshairLibs.GetGlobalScale();

        // color
        effectiveColorAndOpacity = colorAndOpacity;
        if (!ignoreGlobalColor)
            effectiveColorAndOpacity *= CrosshairLibs.GetGlobalTintColor();

        // translation
        effectiveTranslation = (layerTranslation + CrosshairLibs.GetGlobalTranslation()) * crosshairScale;

        // element positioning
        effectiveGap = (recoilResponsePercentageCenterGap * 0.01f * GetWeaponRecoilResponse() + gap) * crosshairScale;

        if (forceEqualDivision)
        {
            stepAngle = 360f / totalElements;
        }

        effectiveAAFilterSize = CrosshairLibs.GetCrosshairAAFilterSize();
    }

    protected void OptimizeMaterialSize()
    {
        materialImageSize.x += Mathf.Abs(effectiveTranslation.x) * 2;
        materialImageSize.y += Mathf.Abs(effectiveTranslation.y) * 2;

        materialImageSize.x = Mathf.Clamp(materialImageSize.x, 2, Screen.width);
        materialImageSize.y = Mathf.Clamp(materialImageSize.y, 2, Screen.height);
    }

    private void UpdateMaterialParameters()
    {
        material.SetFloat("nElementsFloat", totalElements);
        material.SetFloat("stepAngle", stepAngle);
        material.SetFloat("elementRotation", elementAngle + elementAutoRotationValue);
        material.SetFloat("layerRotation", layerAngle + layerAutoRotationValue);
        material.SetFloat("gap", effectiveGap);
        material.SetFloat("AAFilterSize", effectiveAAFilterSize);
        material.SetVector("imageSize", materialImageSize);
        material.SetColor("inColor", effectiveColorAndOpacity);
        material.SetVector("scale", elementScale);
        material.SetVector("translation", effectiveTranslation);
    }

    private void UpdateRectTransform()
    {
        if (childObject == null)
            return;

        var rt = childObject.GetComponent<RectTransform>();
        if (rt == null)
            return;

        // size
        rt.sizeDelta = materialImageSize;
    }

    protected float GetWeaponRecoilResponse()
    {
        return (enableRecoilResponse ? 1f : 0) * CrosshairLibs.GetRecoilMultiplierPixels() * CrosshairLibs.GetRecoil();
    }

    protected void UpdateBaseVariables(
        int totalElements,
        float gap,
        bool forceEqualDivision,
        float stepAngle,
        bool enableRecoilResponse,
        float recoilResponsePercentageCenterGap,
        Vector2 elementScale,
        float elementAngle,
        float layerAngle,
        Vector2 layerTranslation,
        float autoRotationRpmElement,
        float autoRotationRpmLayer,
        Color colorAndOpacity,
        bool ignoreGlobalColor,
        bool fillOutside)
    {
        this.totalElements = totalElements;
        this.gap = gap;
        this.forceEqualDivision = forceEqualDivision;
        this.stepAngle = stepAngle;
        this.enableRecoilResponse = enableRecoilResponse;
        this.recoilResponsePercentageCenterGap = recoilResponsePercentageCenterGap;
        this.elementScale = elementScale;
        this.elementAngle = elementAngle;
        this.layerAngle = layerAngle;
        this.layerTranslation = layerTranslation;
        this.autoRotationRpmElement = autoRotationRpmElement;
        this.autoRotationRpmLayer = autoRotationRpmLayer;
        this.colorAndOpacity = colorAndOpacity;
        this.ignoreGlobalColor = ignoreGlobalColor;
        this.fillOutside = fillOutside;
    }
}
