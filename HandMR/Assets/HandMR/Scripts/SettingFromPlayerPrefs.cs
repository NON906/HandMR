using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hologla;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace HandMR
{
    public class SettingFromPlayerPrefs : MonoBehaviour
    {
        public HandMRManager HandMRManagerObj;
        public GameObject HologlaCameraManagerObj;
        public HandVRMain HandVRMainObj;
        public Transform LeftButton;
        public Transform RightButton;
        public GameObject HandAreaObj = null;

        Fisheye[] fisheyes_;

        void Reset()
        {
            try
            {
                HandMRManagerObj = FindObjectOfType<HandMRManager>();
                HologlaCameraManagerObj = HandMRManagerObj.GetComponentInChildren<HologlaCameraManager>().gameObject;
                HandVRMainObj = HandMRManagerObj.GetComponentInChildren<HandVRMain>();

                var hologlaInput = HandMRManagerObj.GetComponentInChildren<HologlaInput>();
                LeftButton = hologlaInput.LeftButtonComp.transform;
                RightButton = hologlaInput.RightButtonComp.transform;
            }
            catch (NullReferenceException)
            {
                Debug.Log("Setting SettingFromPlayerPrefs is failed.\nPlease setting from Inspector.");
            }

            var handArea = FindObjectOfType<HandArea>();
            if (handArea != null)
            {
                HandAreaObj = handArea.gameObject;
            }
        }

        void Start()
        {
            if (HandMRManagerObj == null || HologlaCameraManagerObj == null || HandVRMainObj == null
                || LeftButton == null || RightButton == null)
            {
                return;
            }

            HandVRMainObj.ShiftX = PlayerPrefs.GetFloat("HandMR_HandPositionX", 0f) * 0.001f;

            HandMRManagerObj.HandDetectionMode =
                (HandMRManager.HandDetection)PlayerPrefs.GetInt("HandMR_HandDetectionMode", (int)HandMRManager.HandDetection.Both);

            int mode = PlayerPrefs.GetInt("HandMR_GoogleMode", 0);
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

                HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();

                if (mode == 1)
                {
                    hologlaCameraManager.SwitchEyeMode(HologlaCameraManager.EyeMode.SingleEye);
                }

                hologlaCameraManager.ApplyIPD(PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", 64f));
                hologlaCameraManager.SwitchViewSize((HologlaCameraManager.ViewSize)PlayerPrefs.GetInt("HandMR_ScreenSize", 0));
            }
            else if (mode == 2)
            {
                HandMRManagerObj.ViewModeChange(HandMRManager.Mode.VR);

                fisheyes_ = HandMRManagerObj.MRObject.GetComponentsInChildren<Fisheye>();
                foreach (Fisheye fisheye in fisheyes_)
                {
                    fisheye.Rate = PlayerPrefs.GetFloat("HandMR_FisheyeRate", 58f) * 0.01f;
                    fisheye.Center = PlayerPrefs.GetFloat("HandMR_FisheyeCenter", 50f) * 0.01f;
                }
                HandMRManagerObj.FieldOfView = PlayerPrefs.GetFloat("HandMR_FisheyeFieldOfView", 90f);

                LeftButton.GetChild(0).GetComponent<Image>().enabled = false;
                RightButton.GetChild(0).GetComponent<Image>().enabled = false;

                HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
                hologlaCameraManager.ApplyIPD(PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", 64f));
                hologlaCameraManager.SwitchViewSize((HologlaCameraManager.ViewSize)PlayerPrefs.GetInt("HandMR_ScreenSize", 0));
            }
            else if (mode == 3)
            {
                HandMRManagerObj.ViewModeChange(HandMRManager.Mode.VRSingle);
            }
            else
            {
                HandVRMainObj.ShiftX = 0f;
                HandMRManagerObj.ViewModeChange(HandMRManager.Mode.AR);
            }

            if (HandAreaObj != null)
            {
                HandAreaObj.SetActive(PlayerPrefs.GetInt("HandMR_HandArea", 1) != 0);
            }
        }
    }
}
