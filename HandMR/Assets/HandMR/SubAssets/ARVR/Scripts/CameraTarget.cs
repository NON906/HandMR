﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

public class CameraTarget : MonoBehaviour
{
    public GameObject BackgroundObj;
    public Transform PoseDriverTrans;
    public Camera[] Cameras;
    public bool TouchReset;

    // トラッキングの位置中心
    Vector3 poseCenter_ = Vector3.zero;
    Quaternion poseCenterRotation_ = Quaternion.identity;

#if DOWNLOADED_ARFOUNDATION
    // 認識した床
    ARPlane plane_ = null;
#endif

    int[] cameraMasks_;

    public bool IsTracking
    {
        private set;
        get;
    } = false;

    void Start()
    {
        cameraMasks_ = new int[Cameras.Length];
        for (int loop = 0; loop < Cameras.Length; loop++)
        {
            cameraMasks_[loop] = Cameras[loop].cullingMask;
            Cameras[loop].cullingMask = 1 << LayerMask.NameToLayer("BackGround");
        }
    }

    void Update()
    {
#if DOWNLOADED_ARFOUNDATION
        if (plane_ == null || (TouchReset && Input.touchCount > 0))
        {
            // 新しい床の取得
            plane_ = null;
            ARPlane newPlane = null;
            float newPlaneDistance = float.PositiveInfinity;
            ARPlane[] planes = FindObjectsOfType<ARPlane>();
            foreach (ARPlane plane in planes)
            {
                if (plane.gameObject.activeSelf)
                {
                    float distance = Vector2.Distance(new Vector2(PoseDriverTrans.position.x, PoseDriverTrans.position.z), new Vector2(plane.transform.position.x, plane.transform.position.z));
                    if (newPlaneDistance >= distance)
                    {
                        newPlane = plane;
                        newPlaneDistance = distance;
                    }
                }
            }

            if (newPlane != null)
            {
                plane_ = newPlane;

                ResetPosition();

                // カメラ画像の切り替え
                if (BackgroundObj != null)
                {
                    StartCoroutine(delayedSubCameraDisable());
                    for (int loop = 0; loop < Cameras.Length; loop++)
                    {
                        Cameras[loop].cullingMask = cameraMasks_[loop];
                    }
                }

                IsTracking = true;
            }

            if (plane_ == null)
            {
                // カメラ画像の切り替え
                if (BackgroundObj != null)
                {
                    BackgroundObj.SetActive(true);
                    for (int loop = 0; loop < Cameras.Length; loop++)
                    {
                        Cameras[loop].cullingMask = 1 << LayerMask.NameToLayer("BackGround");
                    }
                }

                IsTracking = false;
            }
        }

        // 移動
        transform.position = Quaternion.Inverse(poseCenterRotation_) * (PoseDriverTrans.position - poseCenter_);
        transform.rotation = Quaternion.Inverse(poseCenterRotation_) * PoseDriverTrans.rotation;
#endif
    }

    // 位置のリセット
    public void ResetPosition()
    {
#if DOWNLOADED_ARFOUNDATION
        poseCenter_ = PoseDriverTrans.position;
        if (plane_ != null)
        {
            poseCenter_ += new Vector3(0f, plane_.transform.position.y, 0f);
        }

        poseCenterRotation_ = Quaternion.Euler(0f, PoseDriverTrans.eulerAngles.y, 0f);
#endif
    }

    IEnumerator delayedSubCameraDisable()
    {
        yield return null;
        yield return null;

        BackgroundObj.SetActive(false);
    }
}
