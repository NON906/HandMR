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
    public Camera BackGroundCamera;

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
                StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.SolidColor));
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
                StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.Depth));
                break;
        }
    }

    IEnumerator vrCameraSettingCoroutine(CameraClearFlags cameraClearFlags)
    {
        yield return null;

        vrCameraSetting(cameraClearFlags);
    }

    void vrCameraSetting(CameraClearFlags cameraClearFlags)
    {
        Camera cam = VRCamera.GetComponent<Camera>();
        var background = cam.backgroundColor;
        var cullingMask = cam.cullingMask;
        var depth = cam.depth;
        var targetEye = cam.stereoTargetEye;
        cam.CopyFrom(BackGroundCamera);
        cam.clearFlags = cameraClearFlags;
        cam.targetTexture = null;
        cam.backgroundColor = background;
        cam.cullingMask = cullingMask;
        cam.depth = depth;
        cam.stereoTargetEye = targetEye;

        float degRad = cam.fieldOfView * Mathf.Deg2Rad;
        float newDegRad = Mathf.Atan(Mathf.Tan(degRad * 0.5f) * 0.5f) * 2f;
        float newDeg = newDegRad * Mathf.Rad2Deg;
        cam.fieldOfView = newDeg;
    }

    public void ViewModeChange(Mode mode)
    {
        ViewMode = mode;
        viewModeChange();
    }
}
