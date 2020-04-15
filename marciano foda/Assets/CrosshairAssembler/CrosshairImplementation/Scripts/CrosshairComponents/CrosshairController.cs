using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CrosshairController : MonoBehaviour
{
    public enum ListType { None, Advanced, Scopes, Basic };

    [Header("<Crosshair>")]
    public int Index;
    public CrosshairLayerBase[] List;

    [Header("<Transform>")]
    public float GlobalScale = 1;
    public Vector2 GlobalTranslation;

    [Header("<Color>")]
    public Color GlobalTintColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    [Header("<Antialiasing>")]
    public float AAAmountPercentage = 100f;

    [Header("<Recoil>")]
    public float RecoilMultiplierPixels = 10f;

    // private
    private float recoil;

    void OnValidate()
    {
        GlobalScale = Mathf.Max(0, GlobalScale);
        Index = Mathf.Clamp(Index, 0, Mathf.Max(0, List.Length - 1));
        AAAmountPercentage = Mathf.Clamp(AAAmountPercentage, 0, 200);
    }

    void Awake()
    {
        UpdateCrosshairCtonrollerToLib();
    }

    void Start()
    {
        ChangeCrosshair(Index);
    }

    void Update()
    {
        UpdateCrosshairCtonrollerToLib();
        UpdateCrosshairIndex();
    }

    void UpdateCrosshairCtonrollerToLib()
    {
        if (CrosshairLibs.GetCrosshairController() == null)
            CrosshairLibs.SetCrosshairController(this);
    }

    void UpdateCrosshairIndex()
    {
        if (!Application.isPlaying)
            return;

        // check key pressed to change crosshair
        if (List.Length > 0)
        {
            if (Input.GetKeyDown("c"))
            {
                Index = (Index + 1) % List.Length;
                ChangeCrosshair(Index);
            }
            if (Input.GetKeyDown("z"))
            {
                Index = (Index - 1 + List.Length) % List.Length;
                ChangeCrosshair(Index);
            }
        }
    }

    void ChangeCrosshair(int i)
    {
        if (!Application.isPlaying)
            return;

        Debug.Log("Change Crosshair index: " + i);
        if (i >= List.Length || i < 0)
        {
            Debug.LogError("Wrong crosshair index");
            return;
        }

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Instantiate(List[i], transform);
        Index = i;
    }

    public void SetRecoil(float r)
    {
        recoil = r;
    }

    public float GetRecoil()
    {
        return recoil;
    }
}
