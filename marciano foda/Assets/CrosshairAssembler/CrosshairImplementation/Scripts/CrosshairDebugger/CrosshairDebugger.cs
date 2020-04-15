using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairDebugger : MonoBehaviour
{
    [HideInInspector]
    public string AdditionalText;

    private Text debuggerText;
    private bool isActive = true;

    void Update()
    {
        debuggerText = GetComponent<Text>();
        if (debuggerText == null)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
            isActive = !isActive;

        if (!isActive)
        {
            debuggerText.text = "";
            return;
        }

        debuggerText.text = "(Press TAB to toggle debug text.)" + "\n";

        var controller = CrosshairLibs.GetCrosshairController();
        if (controller == null)
        {
            debuggerText.text += "\nCrosshair controller is null.\n";
        }
        else
        {
            int totalCrosshairs = controller.List.Length;
            int currIndex = controller.Index;
            string currName = currIndex < controller.List.Length ? controller.List[controller.Index].name : "";
            debuggerText.text += "\n"
                + "Total crosshairs in the level: " + totalCrosshairs + "\n"
                + "Current crosshair index: " + currIndex + "\n"
                + "Current crosshair name: " + currName + "\n";
        }

        debuggerText.text += "\n"
            + "Crosshair global scale: " + CrosshairLibs.GetGlobalScale() + "\n"
            + "Crosshair recoil: " + CrosshairLibs.GetRecoil().ToString("F6") + "\n"
            + "Crosshair recoil multiplier (pixel): " + CrosshairLibs.GetRecoilMultiplierPixels() + "\n"
            + "Crosshair anti-aliasing filter size: " + CrosshairLibs.GetCrosshairAAFilterSize() + "\n";

        debuggerText.text += AdditionalText + "\n";
        AdditionalText = "";
    }
}
