using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandMRManager : MonoBehaviour
{
    public enum Mode
    {
        MR,
        VR,
        AR
    };

    [SerializeField]
    Mode ViewMode;

    public GameObject[] VRBackgroundObjects;

    public SetParentMainCamera[] Hands;
    public GameObject MRObject;
    public Transform MRCamera;
    public GameObject VRObject;
    public Transform VRCamera;

    void Start()
    {
        viewModeChange();
    }

    void viewModeChange()
    {
        switch (ViewMode)
        {
            case Mode.MR:
                MRObject.SetActive(true);
                VRObject.SetActive(false);
                foreach (SetParentMainCamera hand in Hands)
                {
                    hand.MainCameraTransform = MRCamera;
                }
                foreach (GameObject obj in VRBackgroundObjects)
                {
                    if (obj != null)
                    {
                        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                        {
                            renderer.enabled = false;
                        }
                    }
                }
                break;
            case Mode.VR:
                MRObject.SetActive(false);
                VRObject.SetActive(true);
                foreach (SetParentMainCamera hand in Hands)
                {
                    hand.MainCameraTransform = VRCamera;
                }
                foreach (GameObject obj in VRBackgroundObjects)
                {
                    if (obj != null)
                    {
                        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                        {
                            renderer.enabled = true;
                        }
                    }
                }
                VRObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = true;
                VRObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = !XRSettings.enabled;
                VRCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                break;
            case Mode.AR:
                MRObject.SetActive(false);
                VRObject.SetActive(true);
                foreach (SetParentMainCamera hand in Hands)
                {
                    hand.MainCameraTransform = VRCamera;
                }
                foreach (GameObject obj in VRBackgroundObjects)
                {
                    if (obj != null)
                    {
                        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                        {
                            renderer.enabled = false;
                        }
                    }
                }
                VRObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = false;
                VRObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = true;
                VRCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
                break;
        }
    }

    public void ViewModeChange(Mode mode)
    {
        ViewMode = mode;
        viewModeChange();
    }
}
