using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CrosshairCustomAction : MonoBehaviour
{
    abstract public void UpdateCustomAction();

    void Update()
    {
        UpdateCustomAction();
    }

    protected CrosshairLayerBase GetCrosshairLayer(string layerName)
    {
        var crosshairLayers = GetComponents<CrosshairLayerBase>();
        foreach (var layer in crosshairLayers)
        {
            if (layer.LayerName == layerName)
                return layer;
        }
        return null;
    }

}
