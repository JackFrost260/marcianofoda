using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairLibs
{
    private static CrosshairController crosshairController;
    private static string canvasName = "CrosshairCanvas";

    // Math -----------------------------------------------------
    public static Vector2 Abs(Vector2 v)
    {
        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }

    public static float Hypotenuse(Vector2 size)
    {
        return size.x == 0 && size.y == 0 ? 0 : Mathf.Sqrt(Mathf.Pow(size.x, 2) + Mathf.Pow(size.y, 2));
    }

    // Crosshair -----------------------------------------------------
    public static void SetCrosshairController(CrosshairController c)
    {
        if (c == null)
            return;

        Debug.Log("Crosshair controller is ready now.");
        crosshairController = c;
    }

    public static CrosshairController GetCrosshairController()
    {
        if (crosshairController == null)
        {
            var canvas = GameObject.Find(canvasName);
            if (canvas != null)
                SetCrosshairController(canvas.GetComponent<CrosshairController>());
        }

        if (crosshairController == null)
            Debug.LogError("crosshair controller is null. Add crosshair controller to the " + canvasName);

        return crosshairController;
    }

    public static void SetRecoil(float recoil)
    {
        if (GetCrosshairController() == null)
            return;
        GetCrosshairController().SetRecoil(recoil);
    }

    public static float GetRecoil()
    {
        if (GetCrosshairController() == null)
            return 0;
        return GetCrosshairController().GetRecoil();
    }

    public static float GetGlobalScale()
    {
        if (GetCrosshairController() == null)
            return 1f;
        return GetCrosshairController().GlobalScale;
    }

    public static Vector2 GetGlobalTranslation()
    {
        if (GetCrosshairController() == null)
            return Vector2.zero;
        return GetCrosshairController().GlobalTranslation;
    }

    public static Color GetGlobalTintColor()
    {
        if (GetCrosshairController() == null)
            return Color.white;
        return GetCrosshairController().GlobalTintColor;
    }

    public static float GetCrosshairAAFilterSize()
    {
        if (GetCrosshairController() == null)
            return 1f;
        return GetCrosshairController().AAAmountPercentage * 0.01f;
    }

    public static float GetRecoilMultiplierPixels()
    {
        if (GetCrosshairController() == null)
            return 10f;
        return GetCrosshairController().RecoilMultiplierPixels;
    }

    public static void AddCrosshairDebugText(string additionalText)
    {
        var obj = GameObject.Find("CrosshairDebugger");
        if (obj == null)
            return;

        var dbg = obj.GetComponent<CrosshairDebugger>();
        if (dbg == null)
            return;

        dbg.AdditionalText += additionalText;
    }
}
