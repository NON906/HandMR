using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Dropdown GoogleModeDropdown;
    public Dropdown PhonePositionDropdown;
    public Slider InterpupillaryDistanceSlider;
    public InputField InterpupillaryDistanceInputField;
    public Dropdown ScreenSizeDropdown;
    public Slider HandPositionXSlider;
    public InputField HandPositionXInputField;
    public Slider HandPositionYSlider;
    public InputField HandPositionYInputField;
    //public Slider HandSizeSlider;
    //public InputField HandSizeInputField;
    public Slider FisheyeFieldOfViewSlider;
    public InputField FisheyeFieldOfViewInputField;
    public Slider FisheyeRateSlider;
    public InputField FisheyeRateInputField;
    public Slider FisheyeCenterSlider;
    public InputField FisheyeCenterInputField;
    public Button HandSizeCalibResetButton;
    public Canvas MenuCanvas;

    int googleModeDefault_;
    int phonePositionDefault_;
    float interpupillaryDistanceDefault_;
    int screenSizeDefault_;
    float handPositionXDefault_;
    float handPositionYDefault_;
    //float handSizeDefault_;
    float fisheyeFieldOfViewDefault_;
    float fisheyeRateDefault_;
    float fisheyeCenterDefault_;

    void settingDefaultValues()
    {
        googleModeDefault_ = GoogleModeDropdown.value;
        phonePositionDefault_ = PhonePositionDropdown.value;
        interpupillaryDistanceDefault_ = InterpupillaryDistanceSlider.value;
        screenSizeDefault_ = ScreenSizeDropdown.value;
        handPositionXDefault_ = HandPositionXSlider.value;
        handPositionYDefault_ = HandPositionYSlider.value;
        //handSizeDefault_ = HandSizeSlider.value;
        fisheyeFieldOfViewDefault_ = FisheyeFieldOfViewSlider.value;
        fisheyeRateDefault_ = FisheyeRateSlider.value;
        fisheyeCenterDefault_ = FisheyeCenterSlider.value;
    }

    void saveValues()
    {
        PlayerPrefs.SetInt("HandMR_GoogleMode", GoogleModeDropdown.value);
        PlayerPrefs.SetInt("HandMR_PhonePosition", PhonePositionDropdown.value);
        PlayerPrefs.SetFloat("HandMR_InterpupillaryDistance", InterpupillaryDistanceSlider.value);
        PlayerPrefs.SetInt("HandMR_ScreenSize", ScreenSizeDropdown.value);
        PlayerPrefs.SetFloat("HandMR_HandPositionX", HandPositionXSlider.value);
        PlayerPrefs.SetFloat("HandMR_HandPositionY", HandPositionYSlider.value);
        //PlayerPrefs.SetFloat("HandMR_HandSize", HandSizeSlider.value);
        PlayerPrefs.SetFloat("HandMR_FisheyeFieldOfView", FisheyeFieldOfViewSlider.value);
        PlayerPrefs.SetFloat("HandMR_FisheyeRate", FisheyeRateSlider.value);
        PlayerPrefs.SetFloat("HandMR_FisheyeCenter", FisheyeCenterSlider.value);
        PlayerPrefs.Save();
    }

    void settingEnabled()
    {
        switch (GoogleModeDropdown.value)
        {
            case 0:
                PhonePositionDropdown.interactable = true;
                InterpupillaryDistanceSlider.interactable = true;
                InterpupillaryDistanceInputField.interactable = true;
                ScreenSizeDropdown.interactable = true;
                HandPositionXSlider.interactable = true;
                HandPositionXInputField.interactable = true;
                HandPositionYSlider.interactable = true;
                HandPositionYInputField.interactable = true;
                FisheyeFieldOfViewSlider.interactable = false;
                FisheyeFieldOfViewInputField.interactable = false;
                FisheyeRateSlider.interactable = false;
                FisheyeRateInputField.interactable = false;
                FisheyeCenterSlider.interactable = false;
                FisheyeCenterInputField.interactable = false;
                break;
            case 1:
                PhonePositionDropdown.interactable = true;
                InterpupillaryDistanceSlider.interactable = false;
                InterpupillaryDistanceInputField.interactable = false;
                ScreenSizeDropdown.interactable = true;
                HandPositionXSlider.interactable = true;
                HandPositionXInputField.interactable = true;
                HandPositionYSlider.interactable = true;
                HandPositionYInputField.interactable = true;
                FisheyeFieldOfViewSlider.interactable = false;
                FisheyeFieldOfViewInputField.interactable = false;
                FisheyeRateSlider.interactable = false;
                FisheyeRateInputField.interactable = false;
                FisheyeCenterSlider.interactable = false;
                FisheyeCenterInputField.interactable = false;
                break;
            case 2:
                PhonePositionDropdown.interactable = false;
                InterpupillaryDistanceSlider.interactable = true;
                InterpupillaryDistanceInputField.interactable = true;
                ScreenSizeDropdown.interactable = true;
                HandPositionXSlider.interactable = true;
                HandPositionXInputField.interactable = true;
                HandPositionYSlider.interactable = false;
                HandPositionYInputField.interactable = false;
                FisheyeFieldOfViewSlider.interactable = true;
                FisheyeFieldOfViewInputField.interactable = true;
                FisheyeRateSlider.interactable = true;
                FisheyeRateInputField.interactable = true;
                FisheyeCenterSlider.interactable = true;
                FisheyeCenterInputField.interactable = true;
                break;
            case 3:
                PhonePositionDropdown.interactable = false;
                InterpupillaryDistanceSlider.interactable = false;
                InterpupillaryDistanceInputField.interactable = false;
                ScreenSizeDropdown.interactable = false;
                HandPositionXSlider.interactable = true;
                HandPositionXInputField.interactable = true;
                HandPositionYSlider.interactable = false;
                HandPositionYInputField.interactable = false;
                FisheyeFieldOfViewSlider.interactable = false;
                FisheyeFieldOfViewInputField.interactable = false;
                FisheyeRateSlider.interactable = false;
                FisheyeRateInputField.interactable = false;
                FisheyeCenterSlider.interactable = false;
                FisheyeCenterInputField.interactable = false;
                break;
            case 4:
                PhonePositionDropdown.interactable = false;
                InterpupillaryDistanceSlider.interactable = false;
                InterpupillaryDistanceInputField.interactable = false;
                ScreenSizeDropdown.interactable = false;
                HandPositionXSlider.interactable = false;
                HandPositionXInputField.interactable = false;
                HandPositionYSlider.interactable = false;
                HandPositionYInputField.interactable = false;
                FisheyeFieldOfViewSlider.interactable = false;
                FisheyeFieldOfViewInputField.interactable = false;
                FisheyeRateSlider.interactable = false;
                FisheyeRateInputField.interactable = false;
                FisheyeCenterSlider.interactable = false;
                FisheyeCenterInputField.interactable = false;
                break;
        }
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

        GoogleModeDropdown.value = PlayerPrefs.GetInt("HandMR_GoogleMode", googleModeDefault_);
        PhonePositionDropdown.value = PlayerPrefs.GetInt("HandMR_PhonePosition", phonePositionDefault_);
        float iDistanceValue = PlayerPrefs.GetFloat("HandMR_InterpupillaryDistance", interpupillaryDistanceDefault_);
        InterpupillaryDistanceSlider.value = iDistanceValue;
        InterpupillaryDistanceInputField.text = "" + (int)iDistanceValue;
        ScreenSizeDropdown.value = PlayerPrefs.GetInt("HandMR_ScreenSize", screenSizeDefault_);
        float posXValue = PlayerPrefs.GetFloat("HandMR_HandPositionX", handPositionXDefault_);
        HandPositionXSlider.value = posXValue;
        HandPositionXInputField.text = "" + (int)posXValue;
        float posYValue = PlayerPrefs.GetFloat("HandMR_HandPositionY", handPositionYDefault_);
        HandPositionYSlider.value = posYValue;
        HandPositionYInputField.text = "" + (int)posYValue;
        //float handSizeValue = PlayerPrefs.GetFloat("HandMR_HandSize", handSizeDefault_);
        //HandSizeSlider.value = handSizeValue;
        //HandSizeInputField.text = "" + (int)handSizeValue;
        float fisheyeFieldOfView = PlayerPrefs.GetFloat("HandMR_FisheyeFieldOfView", fisheyeFieldOfViewDefault_);
        FisheyeFieldOfViewSlider.value = fisheyeFieldOfView;
        FisheyeFieldOfViewInputField.text = "" + (int)fisheyeFieldOfView;
        float fisheyeRate = PlayerPrefs.GetFloat("HandMR_FisheyeRate", fisheyeRateDefault_);
        FisheyeRateSlider.value = fisheyeRate;
        FisheyeRateInputField.text = "" + (int)fisheyeRate;
        float fisheyeCenter = PlayerPrefs.GetFloat("HandMR_FisheyeCenter", fisheyeCenterDefault_);
        FisheyeCenterSlider.value = fisheyeCenter;
        FisheyeCenterInputField.text = "" + (int)fisheyeCenter;

        settingEnabled();

        HandSizeCalibResetButton.interactable = PlayerPrefs.HasKey("HandMR_PointLength_15");
    }

    public void ExitButton()
    {
        saveValues();
        Application.Quit();
    }

    public void StartButton()
    {
        saveValues();

        SceneManager.LoadScene("Main");
    }

    public void LicenseButton()
    {
        saveValues();
        SceneManager.LoadScene("License");
    }

    public void ResetButton()
    {
        GoogleModeDropdown.value = googleModeDefault_;
        PhonePositionDropdown.value = phonePositionDefault_;
        float iDistanceValue = interpupillaryDistanceDefault_;
        InterpupillaryDistanceSlider.value = iDistanceValue;
        InterpupillaryDistanceInputField.text = "" + (int)iDistanceValue;
        ScreenSizeDropdown.value = screenSizeDefault_;
        float posXValue = handPositionXDefault_;
        HandPositionXSlider.value = posXValue;
        HandPositionXInputField.text = "" + (int)posXValue;
        float posYValue = handPositionYDefault_;
        HandPositionYSlider.value = posYValue;
        HandPositionYInputField.text = "" + (int)posYValue;
        //float handSizeValue = handSizeDefault_;
        //HandSizeSlider.value = handSizeValue;
        //HandSizeInputField.text = "" + (int)handSizeValue;
        float fisheyeFieldOfView = fisheyeFieldOfViewDefault_;
        FisheyeFieldOfViewSlider.value = fisheyeFieldOfView;
        FisheyeFieldOfViewInputField.text = "" + (int)fisheyeFieldOfView;
        float fisheyeRate = fisheyeRateDefault_;
        FisheyeRateSlider.value = fisheyeRate;
        FisheyeRateInputField.text = "" + (int)fisheyeRate;
        float fisheyeCenter = fisheyeCenterDefault_;
        FisheyeCenterSlider.value = fisheyeCenter;
        FisheyeCenterInputField.text = "" + (int)fisheyeCenter;

        settingEnabled();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitButton();
        }
    }

    public void GoogleModeDropdownChanged(int val)
    {
        settingEnabled();
    }

    public void InterpupillaryDistanceSliderChanged(float val)
    {
        changeSlider(InterpupillaryDistanceInputField, val);
    }

    public void InterpupillaryDistanceInputFieldChanged(string val)
    {
        changeInputField(InterpupillaryDistanceSlider, val);
    }

    public void InterpupillaryDistanceInputFieldEndEdit(string val)
    {
        endEditInputField(InterpupillaryDistanceInputField, InterpupillaryDistanceSlider);
    }

    public void HandPositionXSliderChanged(float val)
    {
        changeSlider(HandPositionXInputField, val);
    }

    public void HandPositionXInputFieldChanged(string val)
    {
        changeInputField(HandPositionXSlider, val);
    }

    public void HandPositionXInputFieldEndEdit(string val)
    {
        endEditInputField(HandPositionXInputField, HandPositionXSlider);
    }

    public void HandPositionYSliderChanged(float val)
    {
        changeSlider(HandPositionYInputField, val);
    }

    public void HandPositionYInputFieldChanged(string val)
    {
        changeInputField(HandPositionYSlider, val);
    }

    public void HandPositionYInputFieldEndEdit(string val)
    {
        endEditInputField(HandPositionYInputField, HandPositionYSlider);
    }

    /*
    public void HandSizeSliderChanged(float val)
    {
        changeSlider(HandSizeInputField, val);
    }

    public void HandSizeInputFieldChanged(string val)
    {
        changeInputField(HandSizeSlider, val);
    }

    public void HandSizeInputFieldEndEdit(string val)
    {
        endEditInputField(HandSizeInputField, HandSizeSlider);
    }
    */

    public void FisheyeFieldOfViewSliderChanged(float val)
    {
        changeSlider(FisheyeFieldOfViewInputField, val);
    }

    public void FisheyeFieldOfViewInputFieldChanged(string val)
    {
        changeInputField(FisheyeFieldOfViewSlider, val);
    }

    public void FisheyeFieldOfViewInputFieldEndEdit(string val)
    {
        endEditInputField(FisheyeFieldOfViewInputField, FisheyeFieldOfViewSlider);
    }

    public void FisheyeRateSliderChanged(float val)
    {
        changeSlider(FisheyeRateInputField, val);
    }

    public void FisheyeRateInputFieldChanged(string val)
    {
        changeInputField(FisheyeRateSlider, val);
    }

    public void FisheyeRateInputFieldEndEdit(string val)
    {
        endEditInputField(FisheyeRateInputField, FisheyeRateSlider);
    }

    public void FisheyeCenterSliderChanged(float val)
    {
        changeSlider(FisheyeCenterInputField, val);
    }

    public void FisheyeCenterInputFieldChanged(string val)
    {
        changeInputField(FisheyeCenterSlider, val);
    }

    public void FisheyeCenterInputFieldEndEdit(string val)
    {
        endEditInputField(FisheyeCenterInputField, FisheyeCenterSlider);
    }

    public void HandSizeCalibButton()
    {
        saveValues();
        SceneManager.LoadScene("HandSizeCalib");
    }

    public void HandSizeCalibResetButtonClick()
    {
        if (PlayerPrefs.HasKey("HandMR_PointLength_15"))
        {
            for (int loop = 0; loop < 5; loop++)
            {
                PlayerPrefs.DeleteKey("HandMR_PointPosX_" + loop);
            }

            for (int loop = 0; loop < 16; loop++)
            {
                PlayerPrefs.DeleteKey("HandMR_PointLength_" + loop);
            }

            PlayerPrefs.Save();
        }

        HandSizeCalibResetButton.interactable = false;
    }
}
