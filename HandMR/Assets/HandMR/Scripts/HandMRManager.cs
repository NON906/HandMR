using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Hologla;

namespace HandMR
{
    public class HandMRManager : MonoBehaviour
    {
        public enum Mode
        {
            MR,
            VR,
            AR,
            VRSingle
        };

        [SerializeField]
        Mode ViewMode;

        public GameObject[] VRBackgroundObjects;

        public SetParentMainCamera[] Hands;
        public GameObject MRObject;
        public Transform MRCamera;
        public GameObject ARObject;
        public Transform ARCamera;
        public Camera BackGroundCamera;
        public GameObject VRSubCamera;
        public GameObject LeftEyeFrame;
        public GameObject RightEyeFrame;
        public Color VRBackColor;

        Fisheye[] fisheyes_;
        public float FieldOfView
        {
            get;
            set;
        }

        void Start()
        {
            fisheyes_ = MRObject.GetComponentsInChildren<Fisheye>();

            viewModeChange();
        }

        void viewModeChange()
        {
            switch (ViewMode)
            {
                case Mode.MR:
                    {
                        MRObject.SetActive(true);
                        ARObject.SetActive(false);
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
                        CameraTarget cameraTarget = MRObject.GetComponentInChildren<CameraTarget>();
                        cameraTarget.BackgroundObj.SetActive(true);
                        VRSubCamera.SetActive(false);

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = MRCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.VR:
                    {
                        MRObject.SetActive(true);
                        MRObject.GetComponentInChildren<HologlaCameraManager>().SwitchViewMode(HologlaCameraManager.ViewMode.VR);
                        ARObject.SetActive(false);
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
                                    renderer.enabled = true;
                                }
                            }
                        }
                        CameraTarget cameraTarget = MRObject.GetComponentInChildren<CameraTarget>();
                        cameraTarget.BackgroundObjAutoDisable = true;
                        cameraTarget.BackgroundObj.SetActive(false);
                        VRSubCamera.SetActive(true);
                        cameraTarget.BackgroundObj = VRSubCamera;
                        ResizeBackGroundQuad resizeBackGroundQuad = MRObject.GetComponentInChildren<ResizeBackGroundQuad>();
                        resizeBackGroundQuad.NoticeTextCenter = false;
                        Fisheye[] fisheyes = MRObject.GetComponentsInChildren<Fisheye>();
                        foreach (Fisheye fisheye in fisheyes)
                        {
                            fisheye.enabled = true;
                            Camera camera = fisheye.GetComponent<Camera>();
                            camera.fieldOfView = 90f;
                        }
                        LeftEyeFrame.SetActive(false);
                        RightEyeFrame.SetActive(false);

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = MRCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.VRSingle:
                    {
                        MRObject.SetActive(false);
                        ARObject.SetActive(true);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = ARCamera;
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
                        ARObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = true;
                        ARObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = true;
                        StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.SolidColor));

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = ARCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
                case Mode.AR:
                    {
                        MRObject.SetActive(false);
                        ARObject.SetActive(true);
                        foreach (SetParentMainCamera hand in Hands)
                        {
                            hand.MainCameraTransform = ARCamera;
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
                        ARObject.GetComponentInChildren<CameraTarget>().BackgroundObjAutoDisable = false;
                        ARObject.GetComponentInChildren<ResizeBackGroundQuad>().NoticeTextCenter = true;
                        StartCoroutine(vrCameraSettingCoroutine(CameraClearFlags.Depth));

                        Canvas[] canvases = FindObjectsOfType<Canvas>();
                        foreach (Canvas canvas in canvases)
                        {
                            if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                            {
                                canvas.worldCamera = ARCamera.GetComponent<Camera>();
                            }
                        }
                    }
                    break;
            }
        }

        IEnumerator vrCameraSettingCoroutine(CameraClearFlags cameraClearFlags)
        {
            yield return null;

            arCameraSetting(cameraClearFlags);
        }

        void arCameraSetting(CameraClearFlags cameraClearFlags)
        {
            Camera cam = ARCamera.GetComponent<Camera>();
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

        void LateUpdate()
        {
            if (ViewMode == Mode.VR)
            {
                foreach (Fisheye fisheye in fisheyes_)
                {
                    Camera camera = fisheye.GetComponent<Camera>();
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    if (!VRSubCamera.activeInHierarchy)
                    {
                        camera.backgroundColor = VRBackColor;
                    }
                    else
                    {
                        camera.backgroundColor = Color.black;
                    }

                    camera.fieldOfView = FieldOfView;
                }
            }
        }

        public Transform GetCameraTransform()
        {
            Transform ret = null;

            switch (ViewMode)
            {
                case Mode.MR:
                case Mode.VR:
                    ret = MRCamera;
                    break;
                case Mode.VRSingle:
                case Mode.AR:
                    ret = ARCamera;
                    break;
            }

            return ret;
        }
    }
}
