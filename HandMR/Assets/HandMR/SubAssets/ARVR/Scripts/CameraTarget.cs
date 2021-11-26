using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
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

        public Transform CenterTransform
        {
            get;
            set;
        } = null;

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

        public static bool IsTakeOver
        {
            get;
            set;
        } = false;
        static Vector3 takeOverPosition_;
        static Quaternion takeOverRotation_;
        bool takeOverFlag_ = false;

        void OnDestroy()
        {
            if (IsTakeOver)
            {
                takeOverPosition_ = transform.position;
                takeOverRotation_ = transform.rotation;
            }
            else
            {
#if DOWNLOADED_ARFOUNDATION
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
#endif
            }
        }

        void deviceUpdate()
        {
#if DOWNLOADED_ARFOUNDATION
            var state = new HandMRHMDDeviceState();
            state.isTracked = IsTracking;
            state.position = transform.position;
            state.rotation = transform.rotation;
            state.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);
            InputSystem.QueueStateEvent(HandMRHMDDevice.current, state);
#endif
        }

        void Awake()
        {
            if (IsTakeOver)
            {
                poseCenter_ = -takeOverPosition_;
                poseCenterRotation_ = Quaternion.Euler(0f, -takeOverRotation_.eulerAngles.y, 0f);

                takeOverFlag_ = true;
            }
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

            if (!PlaneDetectEnabled || takeOverFlag_)
            {
                // カメラ画像の切り替え
                disableCameraImage();
            }

#if DOWNLOADED_ARFOUNDATION
            if (HandMRHMDDevice.current == null)
            {
                InputSystem.AddDevice<HandMRHMDDevice>();
            }
#endif
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
                if (CenterTransform != null)
                {
                    transform.position = CenterTransform.rotation * PoseDriverTrans.position + CenterTransform.position;
                    transform.rotation = CenterTransform.rotation * PoseDriverTrans.rotation;
                }
                else
                {
                    transform.position = PoseDriverTrans.position;
                    transform.rotation = PoseDriverTrans.rotation;
                }

                return;
            }

#if DOWNLOADED_ARFOUNDATION
            bool touched = Pointer.current.press.wasPressedThisFrame;// || Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            if (plane_ == null || (TouchReset && touched))
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

                    if (!takeOverFlag_)
                    {
                        ResetPosition();

                        // カメラ画像の切り替え
                        disableCameraImage();
                    }
                    takeOverFlag_ = false;
                }

                if (plane_ == null && !takeOverFlag_)
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
            if (CenterTransform != null)
            {
                poseCenterRotation_ *= Quaternion.Inverse(CenterTransform.rotation);
                CenterTransform.rotation = Quaternion.identity;

                transform.position = CenterTransform.rotation * Quaternion.Inverse(poseCenterRotation_) * (PoseDriverTrans.position - poseCenter_) + CenterTransform.position;
                transform.rotation = CenterTransform.rotation * Quaternion.Inverse(poseCenterRotation_) * PoseDriverTrans.rotation;
            }
            else
            {
                transform.position = Quaternion.Inverse(poseCenterRotation_) * (PoseDriverTrans.position - poseCenter_);
                transform.rotation = Quaternion.Inverse(poseCenterRotation_) * PoseDriverTrans.rotation;
            }
#endif
            deviceUpdate();
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
