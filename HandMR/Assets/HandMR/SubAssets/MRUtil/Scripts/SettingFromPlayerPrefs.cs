using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOWNLOADED_HOLOGLA
using Hologla;
#endif

public class SettingFromPlayerPrefs : MonoBehaviour
{
    public GameObject HologlaCameraManagerObj;
    public HandVRMain HandVRMainObj;
    public Transform LeftButton;
    public Transform RightButton;

    void Start()
    {
        HandVRMainObj.ShiftX = PlayerPrefs.GetFloat("HandMR_HandPositionX", 0f) * 0.001f;
        HandVRMainObj.ShiftY = PlayerPrefs.GetFloat("HandMR_HandPositionY", 0f) * 0.001f;

        if (PlayerPrefs.GetInt("HandMR_PhonePosition", 0) == 1)
        {
            HandVRMainObj.ShiftY *= -1f;
            LeftButton.localScale = new Vector3(LeftButton.localScale.x, -LeftButton.localScale.y, LeftButton.localScale.z);
            RightButton.localScale = new Vector3(RightButton.localScale.x, -RightButton.localScale.y, RightButton.localScale.z);
        }

#if DOWNLOADED_HOLOGLA
        HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();

        if (PlayerPrefs.GetInt("HandMR_GoogleMode", 0) == 1)
        {
            hologlaCameraManager.SwitchEyeMode(HologlaCameraManager.EyeMode.SingleEye);
        }

        hologlaCameraManager.ApplyIPD(PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", 64f));
        hologlaCameraManager.SwitchViewSize((HologlaCameraManager.ViewSize)PlayerPrefs.GetInt("HandMR_ScreenSize", 0));
#endif
    }
}
