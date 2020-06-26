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
    public Slider HandSizeSlider;
    public InputField HandSizeInputField;
    public Canvas MenuCanvas;

    int googleModeDefault_;
    int phonePositionDefault_;
    float interpupillaryDistanceDefault_;
    int screenSizeDefault_;
    float HandPositionXDefault_;
    float HandPositionYDefault_;
    float HandSizeDefault_;

    void settingDefaultValues()
    {
        googleModeDefault_ = GoogleModeDropdown.value;
        phonePositionDefault_ = PhonePositionDropdown.value;
        interpupillaryDistanceDefault_ = InterpupillaryDistanceSlider.value;
        screenSizeDefault_ = ScreenSizeDropdown.value;
        HandPositionXDefault_ = HandPositionXSlider.value;
        HandPositionYDefault_ = HandPositionYSlider.value;
        HandSizeDefault_ = HandSizeSlider.value;
    }

    void saveValues()
    {
        PlayerPrefs.SetInt("HandMR_GoogleMode", GoogleModeDropdown.value);
        PlayerPrefs.SetInt("HandMR_PhonePosition", PhonePositionDropdown.value);
        PlayerPrefs.SetFloat("HandMR_InterpupillaryDistance", InterpupillaryDistanceSlider.value);
        PlayerPrefs.SetInt("HandMR_ScreenSize", ScreenSizeDropdown.value);
        PlayerPrefs.SetFloat("HandMR_HandPositionX", HandPositionXSlider.value);
        PlayerPrefs.SetFloat("HandMR_HandPositionY", HandPositionYSlider.value);
        PlayerPrefs.SetFloat("HandMR_HandSize", HandSizeSlider.value);
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
                break;
            case 2:
            case 3:
                PhonePositionDropdown.interactable = false;
                InterpupillaryDistanceSlider.interactable = false;
                InterpupillaryDistanceInputField.interactable = false;
                ScreenSizeDropdown.interactable = false;
                HandPositionXSlider.interactable = true;
                HandPositionXInputField.interactable = true;
                HandPositionYSlider.interactable = false;
                HandPositionYInputField.interactable = false;
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
        float posXValue = PlayerPrefs.GetFloat("HandMR_HandPositionX", HandPositionXDefault_);
        HandPositionXSlider.value = posXValue;
        HandPositionXInputField.text = "" + (int)posXValue;
        float posYValue = PlayerPrefs.GetFloat("HandMR_HandPositionY", HandPositionYDefault_);
        HandPositionYSlider.value = posYValue;
        HandPositionYInputField.text = "" + (int)posYValue;
        float handSizeValue = PlayerPrefs.GetFloat("HandMR_HandSize", HandSizeDefault_);
        HandSizeSlider.value = handSizeValue;
        HandSizeInputField.text = "" + (int)handSizeValue;

        settingEnabled();
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
        float posXValue = HandPositionXDefault_;
        HandPositionXSlider.value = posXValue;
        HandPositionXInputField.text = "" + (int)posXValue;
        float posYValue = HandPositionYDefault_;
        HandPositionYSlider.value = posYValue;
        HandPositionYInputField.text = "" + (int)posYValue;
        float handSizeValue = HandSizeDefault_;
        HandSizeSlider.value = handSizeValue;
        HandSizeInputField.text = "" + (int)handSizeValue;

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
}
