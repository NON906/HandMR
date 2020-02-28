using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class SettingFromPlayerPrefsVR : MonoBehaviour
{
    public HandVRMain HandVRMainObj;

    IEnumerator Start()
    {
        HandVRMainObj.ShiftX = PlayerPrefs.GetFloat("HandMR_HandPositionX", 0f) * 0.001f;

        if (PlayerPrefs.GetInt("HandMR_GoogleMode", 2) != 3)
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
    }
}
