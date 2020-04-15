using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class CrosshairLayerManager : MonoBehaviour
{
    public Sprite ThumbnailImage;

    private List<int> instanceIDListOfLayer;

    void Awake()
    {
        instanceIDListOfLayer = new List<int>();
    }

    void Start()
    {
        var crosshairLayers = GetComponents<CrosshairLayerBase>();
        foreach (var scr in crosshairLayers)
        {
            scr.CreateCrosshair();
        }

        if (!Application.isPlaying)
            Update();
    }

    void Update()
    {
        transform.localPosition = Vector3.zero; // To make the position center always. The position should be controled by the global translation only.

        CleanUpCrosshairsAndResetInOrder();

        UpdateThumbnailImageInEditMode();
    }

    void CleanUpCrosshairsAndResetInOrder()
    {
        var crosshairLayers = GetComponents<CrosshairLayerBase>();
        bool needReset = false;

        if (IsAnyLayerChanged(crosshairLayers))
        {
            DestroyChildren(); // To destroy removed crosshairs.
            needReset = true;
        }
        else
        {
            foreach (var scr in crosshairLayers)
            {
                if (scr.IsNeedReset())
                {
                    needReset = true;
                    break;
                }
            }
        }

        // Destroy, create and update crosshairs in order.
        if (needReset)
        {
            foreach (var scr in crosshairLayers)
            {
                scr.ResetCrosshair();
            }
        }
    }

    void DestroyChildren()
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
    }

    bool IsAnyLayerChanged(CrosshairLayerBase[] scripts)
    {
        bool isChanged = false;
        if (scripts.Length != instanceIDListOfLayer.Count)
            isChanged = true;
        else
        {
            for (int i = 0; i < scripts.Length; ++i)
            {
                if (scripts[i].GetInstanceID() != instanceIDListOfLayer[i])
                {
                    isChanged = true;
                    break;
                }
            }
        }

        instanceIDListOfLayer.Clear();
        foreach (var src in scripts)
        {
            instanceIDListOfLayer.Add(src.GetInstanceID());
        }

        return isChanged;
    }

    void UpdateThumbnailImageInEditMode()
    {
        if (Application.isPlaying)
            return;

        foreach (Transform child in transform)
        {
            if (child.name == "Thumbnail")
                return;
        }

        GameObject thumbnail = new GameObject();
        thumbnail.transform.SetParent(transform);
        thumbnail.name = "Thumbnail";
        var sr = thumbnail.AddComponent<SpriteRenderer>();

        if ((sr.sprite == null && ThumbnailImage != null)
            || (sr.sprite != null && ThumbnailImage != null && ThumbnailImage.GetInstanceID() != sr.sprite.GetInstanceID()))
        {
            sr.sprite = ThumbnailImage;
        }
    }
}
