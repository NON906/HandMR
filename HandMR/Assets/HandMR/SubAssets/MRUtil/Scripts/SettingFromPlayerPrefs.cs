using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOWNLOADED_HOLOGLA
using Hologla;
#endif
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SettingFromPlayerPrefs : MonoBehaviour
{
    public HandMRManager HandMRManagerObj;
    public GameObject HologlaCameraManagerObj;
    public HandVRMain HandVRMainObj;
    public Transform LeftButton;
    public Transform RightButton;

    IEnumerator Start()
    {
        HandVRMainObj.ShiftX = PlayerPrefs.GetFloat("HandMR_HandPositionX", 0f) * 0.001f;
        HandVRMainObj.HandSize = PlayerPrefs.GetFloat("HandMR_HandSize", 130f) * 0.001f;

        int mode = PlayerPrefs.GetInt("HandMR_GoogleMode", 0);
        if (mode == 2)
        {
            if (XRSettings.loadedDeviceName != XRSettings.supportedDevices[1] || !XRSettings.enabled)
            {
                XRSettings.LoadDeviceByName(XRSettings.supportedDevices[1]);
                yield return null;
                XRSettings.enabled = true;
                yield return null;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            XRSettings.enabled = false;
        }

        if (mode <= 1)
        {
            HandMRManagerObj.ViewModeChange(HandMRManager.Mode.MR);

            if (PlayerPrefs.GetInt("HandMR_PhonePosition", 0) == 0)
            {
                HandVRMainObj.ShiftY = PlayerPrefs.GetFloat("HandMR_HandPositionY", 0f) * 0.001f;
            }
            else
            {
                HandVRMainObj.ShiftY = PlayerPrefs.GetFloat("HandMR_HandPositionY", 0f) * -0.001f;
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
        else
        {
            HandMRManagerObj.ViewModeChange(HandMRManager.Mode.VR);
        }
    }
}
