#if ENABLE_URP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Hologla;

namespace HandMR
{
    public class VRSettingsURP : MonoBehaviour
    {
        public Slider InterpupillaryDistanceSlider;
        public InputField InterpupillaryDistanceInputField;
        public Dropdown ScreenSizeDropdown;
        public Slider FisheyeFieldOfViewSlider;
        public InputField FisheyeFieldOfViewInputField;
        public Slider FisheyeRateSlider;
        public InputField FisheyeRateInputField;
        public Slider FisheyeCenterSlider;
        public InputField FisheyeCenterInputField;
        public Slider ScaleSlider;
        public InputField ScaleInputField;

        public HandMRManager HandMRManagerObj;
        public GameObject HologlaCameraManagerObj;

        float interpupillaryDistanceDefault_;
        int screenSizeDefault_;
        float fisheyeFieldOfViewDefault_;
        float fisheyeRateDefault_;
        float fisheyeCenterDefault_;
        float scaleDefault_;

        FisheyeURP[] fisheyes_;

        void settingDefaultValues()
        {
            interpupillaryDistanceDefault_ = InterpupillaryDistanceSlider.value;
            screenSizeDefault_ = ScreenSizeDropdown.value;
            fisheyeFieldOfViewDefault_ = FisheyeFieldOfViewSlider.value;
            fisheyeRateDefault_ = FisheyeRateSlider.value;
            fisheyeCenterDefault_ = FisheyeCenterSlider.value;
            scaleDefault_ = ScaleSlider.value;
        }

        void saveValues()
        {
            PlayerPrefs.SetFloat("HandMR_InterpupillaryDistance", InterpupillaryDistanceSlider.value);
            PlayerPrefs.SetInt("HandMR_ScreenSize", ScreenSizeDropdown.value);
            PlayerPrefs.SetFloat("HandMR_FisheyeFieldOfView", FisheyeFieldOfViewSlider.value);
            PlayerPrefs.SetFloat("HandMR_URP_Intensity", FisheyeRateSlider.value);
            PlayerPrefs.SetFloat("HandMR_FisheyeCenter", FisheyeCenterSlider.value);
            PlayerPrefs.SetFloat("HandMR_URP_Scale", ScaleSlider.value);
            PlayerPrefs.Save();
        }

        void changeSlider(InputField inputField, float val)
        {
            inputField.text = "" + (int)val;
        }

        void changeInputField(Slider slider, string text)
        {
            float val;
            if (float.TryParse(text, out val))
            {
                slider.value = val;
            }
        }

        void endEditInputField(InputField inputField, Slider slider)
        {
            float val;
            if (!float.TryParse(inputField.text, out val))
            {
                inputField.text = "" + (int)slider.value;
            }
        }

        void Start()
        {
            XRSettings.enabled = false;

            settingDefaultValues();

            fisheyes_ = HandMRManagerObj.MRObject.GetComponentsInChildren<FisheyeURP>();

            float iDistanceValue = PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", interpupillaryDistanceDefault_);
            InterpupillaryDistanceSlider.value = iDistanceValue;
            InterpupillaryDistanceInputField.text = "" + (int)iDistanceValue;
            ScreenSizeDropdown.value = PlayerPrefs.GetInt("HandMR_ScreenSize", screenSizeDefault_);

            float fisheyeFieldOfView = PlayerPrefs.GetFloat("HandMR_FisheyeFieldOfView", fisheyeFieldOfViewDefault_);
            FisheyeFieldOfViewSlider.value = fisheyeFieldOfView;
            FisheyeFieldOfViewInputField.text = "" + (int)fisheyeFieldOfView;
            float fisheyeRate = PlayerPrefs.GetFloat("HandMR_URP_Intensity", fisheyeRateDefault_);
            FisheyeRateSlider.value = fisheyeRate;
            FisheyeRateInputField.text = "" + (int)fisheyeRate;
            float fisheyeCenter = PlayerPrefs.GetFloat("HandMR_FisheyeCenter", fisheyeCenterDefault_);
            FisheyeCenterSlider.value = fisheyeCenter;
            FisheyeCenterInputField.text = "" + (int)fisheyeCenter;
            float scale = PlayerPrefs.GetFloat("HandMR_URP_Scale", scaleDefault_);
            ScaleSlider.value = scale;
            ScaleInputField.text = "" + (int)scale;

            HandMRManagerObj.ViewModeChange(HandMRManager.Mode.VR);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Intensity = PlayerPrefs.GetFloat("HandMR_URP_Intensity", fisheyeRateDefault_) * 0.01f;
                fisheye.CenterPosition = PlayerPrefs.GetFloat("HandMR_FisheyeCenter", fisheyeCenterDefault_) * 0.01f;
                fisheye.Scale = PlayerPrefs.GetFloat("HandMR_URP_Scale", scaleDefault_) * 0.01f;
            }
            HandMRManagerObj.FieldOfView = PlayerPrefs.GetFloat("HandMR_FisheyeFieldOfView", fisheyeFieldOfViewDefault_);

            HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
            hologlaCameraManager.ApplyIPD(PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", interpupillaryDistanceDefault_));
            hologlaCameraManager.SwitchViewSize((HologlaCameraManager.ViewSize)PlayerPrefs.GetInt("HandMR_ScreenSize", screenSizeDefault_));
        }

        public void ResetButton()
        {
            float iDistanceValue = interpupillaryDistanceDefault_;
            InterpupillaryDistanceSlider.value = iDistanceValue;
            InterpupillaryDistanceInputField.text = "" + (int)iDistanceValue;
            ScreenSizeDropdown.value = screenSizeDefault_;

            float fisheyeFieldOfView = fisheyeFieldOfViewDefault_;
            FisheyeFieldOfViewSlider.value = fisheyeFieldOfView;
            FisheyeFieldOfViewInputField.text = "" + (int)fisheyeFieldOfView;
            float fisheyeRate = fisheyeRateDefault_;
            FisheyeRateSlider.value = fisheyeRate;
            FisheyeRateInputField.text = "" + (int)fisheyeRate;
            float fisheyeCenter = fisheyeCenterDefault_;
            FisheyeCenterSlider.value = fisheyeCenter;
            FisheyeCenterInputField.text = "" + (int)fisheyeCenter;
            float scale = scaleDefault_;
            ScaleSlider.value = scale;
            ScaleInputField.text = "" + (int)scale;
        }

        public void InterpupillaryDistanceSliderChanged(float val)
        {
            changeSlider(InterpupillaryDistanceInputField, val);

            HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
            hologlaCameraManager.ApplyIPD(val);
        }

        public void InterpupillaryDistanceInputFieldChanged(string val)
        {
            changeInputField(InterpupillaryDistanceSlider, val);

            HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
            hologlaCameraManager.ApplyIPD(InterpupillaryDistanceSlider.value);
        }

        public void InterpupillaryDistanceInputFieldEndEdit(string val)
        {
            endEditInputField(InterpupillaryDistanceInputField, InterpupillaryDistanceSlider);

            HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
            hologlaCameraManager.ApplyIPD(InterpupillaryDistanceSlider.value);
        }

        public void ScreenSizeDropdownChanged(int val)
        {
            HologlaCameraManager hologlaCameraManager = HologlaCameraManagerObj.GetComponent<HologlaCameraManager>();
            hologlaCameraManager.SwitchViewSize((HologlaCameraManager.ViewSize)val);
        }

        public void FisheyeFieldOfViewSliderChanged(float val)
        {
            changeSlider(FisheyeFieldOfViewInputField, val);

            HandMRManagerObj.FieldOfView = val;
        }

        public void FisheyeFieldOfViewInputFieldChanged(string val)
        {
            changeInputField(FisheyeFieldOfViewSlider, val);

            HandMRManagerObj.FieldOfView = FisheyeFieldOfViewSlider.value;
        }

        public void FisheyeFieldOfViewInputFieldEndEdit(string val)
        {
            endEditInputField(FisheyeFieldOfViewInputField, FisheyeFieldOfViewSlider);

            HandMRManagerObj.FieldOfView = FisheyeFieldOfViewSlider.value;
        }

        public void FisheyeRateSliderChanged(float val)
        {
            changeSlider(FisheyeRateInputField, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Intensity = val * 0.01f;
            }
        }

        public void FisheyeRateInputFieldChanged(string val)
        {
            changeInputField(FisheyeRateSlider, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Intensity = FisheyeRateSlider.value * 0.01f;
            }
        }

        public void FisheyeRateInputFieldEndEdit(string val)
        {
            endEditInputField(FisheyeRateInputField, FisheyeRateSlider);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Intensity = FisheyeRateSlider.value * 0.01f;
            }
        }

        public void FisheyeCenterSliderChanged(float val)
        {
            changeSlider(FisheyeCenterInputField, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.CenterPosition = val * 0.01f;
            }
        }

        public void FisheyeCenterInputFieldChanged(string val)
        {
            changeInputField(FisheyeCenterSlider, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.CenterPosition = FisheyeCenterSlider.value * 0.01f;
            }
        }

        public void FisheyeCenterInputFieldEndEdit(string val)
        {
            endEditInputField(FisheyeCenterInputField, FisheyeCenterSlider);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.CenterPosition = FisheyeCenterSlider.value * 0.01f;
            }
        }

        public void ScaleSliderChanged(float val)
        {
            changeSlider(ScaleInputField, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Scale = val * 0.01f;
            }
        }

        public void ScaleInputFieldChanged(string val)
        {
            changeInputField(ScaleSlider, val);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Scale = ScaleSlider.value * 0.01f;
            }
        }

        public void ScaleInputFieldEndEdit(string val)
        {
            endEditInputField(ScaleInputField, ScaleSlider);

            foreach (FisheyeURP fisheye in fisheyes_)
            {
                fisheye.Scale = ScaleSlider.value * 0.01f;
            }
        }

        void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                BackButton();
            }
        }

        public void BackButton()
        {
            saveValues();
            SceneManager.LoadScene("Menu");
        }
    }
}

#endif