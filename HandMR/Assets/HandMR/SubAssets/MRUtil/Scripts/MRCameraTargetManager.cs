using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MRCameraTargetManager : MonoBehaviour
{
    public CameraTarget CameraTargetObject;
    public ARKitCameraTarget ARKitCameraTargetObject;
    public Camera[] Cameras;
    public Transform CameraTransform;

    Vector3 startPosition_;
    float startEulerAngleY_;

    IEnumerator Start()
    {
        if (CameraTransform == null)
        {
            CameraTransform = Camera.main.transform;
        }

        if (CameraTargetObject != null)
        {
            CameraTargetObject.BackGround.transform.parent = CameraTransform;
        }
        if (ARKitCameraTargetObject != null)
        {
            ARKitCameraTargetObject.BackGround.transform.parent = CameraTransform;
        }

        yield return null;

        int[] cameraDefaultLayers;

        cameraDefaultLayers = new int[Cameras.Length];
        for (int loop = 0; loop < Cameras.Length; loop++)
        {
            cameraDefaultLayers[loop] = Cameras[loop].cullingMask;
            if (CameraTargetObject != null)
            {
                Cameras[loop].cullingMask = 1 << CameraTargetObject.BackGround.layer;
            }
        }

        while (CameraTargetObject != null && CameraTargetObject.BackGround.activeSelf)
        {
            yield return null;
        }
        while (ARKitCameraTargetObject != null && ARKitCameraTargetObject.BackGround.activeSelf)
        {
            yield return null;
        }

        if (CameraTargetObject != null)
        {
            startPosition_ = CameraTargetObject.transform.position;
            startEulerAngleY_ = CameraTargetObject.transform.eulerAngles.y;
            CameraTargetObject.BackGround.SetActive(false);
        }
        if (ARKitCameraTargetObject != null)
        {
            startPosition_ = ARKitCameraTargetObject.transform.position;
            startEulerAngleY_ = ARKitCameraTargetObject.transform.eulerAngles.y;
            ARKitCameraTargetObject.BackGround.SetActive(false);
        }

        ResetPosition();

        for (int loop = 0; loop < Cameras.Length; loop++)
        {
            Cameras[loop].cullingMask = cameraDefaultLayers[loop];
        }
    }

    public void ResetPosition()
    {
        CameraTransform.parent.eulerAngles = new Vector3(0f, startEulerAngleY_ - CameraTransform.localEulerAngles.y, 0f);
        CameraTransform.parent.position = new Vector3(CameraTransform.parent.position.x - CameraTransform.position.x, startPosition_.y, CameraTransform.parent.position.z - CameraTransform.position.z);
    }
}
