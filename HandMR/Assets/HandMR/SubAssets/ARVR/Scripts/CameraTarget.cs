using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace HandMR
{
    public class CameraTarget : MonoBehaviour
    {
        public GameObject BackgroundObj;
        public Transform PoseDriverTrans;
        public Camera[] Cameras;
        public bool TouchReset;
        public bool BackgroundObjAutoDisable = true;

        public bool PlaneDetectEnabled
        {
            get;
            set;
        } = true;

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

        void OnDestroy()
        {
#if DOWNLOADED_ARFOUNDATION
            LoaderUtility.Deinitialize();
            LoaderUtility.Initialize();
#endif
        }

        void Start()
        {
            if (Cameras == null)
            {
                Cameras = new Camera[0];
            }

            cameraMasks_ = new int[Cameras.Length];
            for (int loop = 0; loop < Cameras.Length; loop++)
            {
                cameraMasks_[loop] = Cameras[loop].cullingMask;
                Cameras[loop].cullingMask = 1 << LayerMask.NameToLayer("BackGround");
            }

            if (!PlaneDetectEnabled)
            {
                // カメラ画像の切り替え
                disableCameraImage();
            }
        }

        void disableCameraImage()
        {
#if DOWNLOADED_ARFOUNDATION
            if (BackgroundObj != null)
            {
                if (BackgroundObjAutoDisable)
                {
                    StartCoroutine(delayedSubCameraDisable());
                    for (int loop = 0; loop < Cameras.Length; loop++)
                    {
                        Cameras[loop].cullingMask = cameraMasks_[loop];
                    }
                }
                else
                {
                    BackgroundObj.GetComponent<Camera>().depth = -51;

                    ResizeBackGroundQuad resizeBackGroundQuad = BackgroundObj.GetComponentInChildren<ResizeBackGroundQuad>();
                    if (resizeBackGroundQuad != null)
                    {
                        resizeBackGroundQuad.FieldOfView =
                            FindObjectOfType<ARCameraManager>().GetComponent<Camera>().fieldOfView;
                        resizeBackGroundQuad.Resize();
                    }
                }
            }

            IsTracking = true;
#endif
        }

        void Update()
        {
            if (!PlaneDetectEnabled)
            {
                // 移動
                transform.position = PoseDriverTrans.position;
                transform.rotation = PoseDriverTrans.rotation;

                return;
            }

#if DOWNLOADED_ARFOUNDATION
            if (plane_ == null || (TouchReset && Input.touchCount > 0))
            {
                // 新しい床の取得
                plane_ = null;
                ARPlane newPlane = null;
                float newPlaneDistance = float.PositiveInfinity;
                ARPlane[] planes = FindObjectsOfType<ARPlane>();
                if (planes == null)
                {
                    planes = new ARPlane[0];
                }
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
                    disableCameraImage();
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
                        if (!BackgroundObjAutoDisable)
                        {
                            BackgroundObj.GetComponent<Camera>().depth = 0;
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
}
