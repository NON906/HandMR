using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if DOWNLOADED_ARFOUNDATION
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
#endif
using UnityEngine.XR;

namespace HandMR
{
    public class HandVRSphereHand : MonoBehaviour
    {
        const bool KEEP_TRACKING_GRABED = true;
        const float BASE_ANGLE = -5f;

        public enum EitherHand
        {
            Left,
            Right
        }

        public EitherHand ThisEitherHand = EitherHand.Left;

        bool isSetDefaultRotation_ = false;
        Quaternion defaultRotation_;
        public Quaternion DefaultRotation
        {
            get
            {
                if (!isSetDefaultRotation_)
                {
                    defaultRotation_ = GetComponentInParent<HandMRManager>().DefaultRotation;
                    isSetDefaultRotation_ = true;
                }

                return defaultRotation_;
            }
        }

        Transform[] fingers_ = new Transform[21];
        bool[] fingerTracking_ = new bool[5];
        bool[] fingerOpened_ = new bool[5];
        HandVRMain handVRMain_;
        HandMRManager handMRManager_;
#if DOWNLOADED_ARFOUNDATION
        HandMRInputDeviceState inputState_ = new HandMRInputDeviceState();
#endif

        public Vector3 HandCenterPosition
        {
            get
            {
                return (fingers_[0].position + fingers_[5].position + fingers_[17].position) / 3f;
            }
        }

        public Vector3 HandDirection
        {
            get;
            private set;
        }

        public bool IsTrackingHand
        {
            get
            {
                for (int loop = 0; loop < 5; loop++)
                {
                    if (!fingerTracking_[loop])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsGrab
        {
            get
            {
                var gestures = handVRMain_.GetGestures(handVRMain_.GetIdFromHandednesses(ThisEitherHand));
                return gestures[(int)HandVRMain.GestureType.Open] < gestures[(int)HandVRMain.GestureType.Close]
                    || gestures[(int)HandVRMain.GestureType.Open] < gestures[(int)HandVRMain.GestureType.Grab];
            }
        }

        bool calcFingerOpened(Vector3 rootVec, Vector3 tipVec, float targetCos)
        {
            rootVec.Normalize();
            tipVec.Normalize();

            float cos = rootVec.x * tipVec.x + rootVec.y * tipVec.y + rootVec.z * tipVec.z;

            return cos > targetCos;
        }

        void startInputDevice()
        {
#if DOWNLOADED_ARFOUNDATION
            if (XRController.leftHand == null && ThisEitherHand == EitherHand.Left)
            {
                var inputDevice = InputSystem.AddDevice<HandMRInputDevice>();
                InputSystem.AddDeviceUsage(inputDevice, UnityEngine.InputSystem.CommonUsages.LeftHand);
            }
            if (XRController.rightHand == null && ThisEitherHand == EitherHand.Right)
            {
                var inputDevice = InputSystem.AddDevice<HandMRInputDevice>();
                InputSystem.AddDeviceUsage(inputDevice, UnityEngine.InputSystem.CommonUsages.RightHand);
            }
#endif
        }

        void Start()
        {
            handVRMain_ = FindObjectOfType<HandVRMain>();
            handMRManager_ = FindObjectOfType<HandMRManager>();

            foreach (Transform child in transform)
            {
                HandVRPosition posObj = child.GetComponent<HandVRPosition>();
                if (posObj != null)
                {
                    fingers_[posObj.Index] = child;
                    posObj.ThisEitherHand = ThisEitherHand;
                }
            }

            startInputDevice();
        }

        void inputUpdate()
        {
#if DOWNLOADED_ARFOUNDATION
            int id = handVRMain_.GetIdFromHandednesses(ThisEitherHand);
            if (!IsTrackingHand || id < 0)
            {
                if (!KEEP_TRACKING_GRABED || !inputState_.isGrab)
                {
                    inputState_.isTracked = false;
                }
                InputSystem.QueueStateEvent(ThisEitherHand == EitherHand.Left ? XRController.leftHand : XRController.rightHand, inputState_);
                return;
            }

            inputState_.isTracked = true;

            var gestures = handVRMain_.GetGestures(id);
            inputState_.isGrab = gestures[(int)HandVRMain.GestureType.Open] < gestures[(int)HandVRMain.GestureType.Close]
                || gestures[(int)HandVRMain.GestureType.Open] < gestures[(int)HandVRMain.GestureType.Grab];

            int gesturesMaxId = 0;
            float maxValue = 0f;
            for (int gestureId = 0; gestureId < gestures.Length; gestureId++)
            {
                if (maxValue < gestures[gestureId])
                {
                    gesturesMaxId = gestureId;
                    maxValue = gestures[gestureId];
                }
            }
            inputState_.gestures = (uint)1 << gesturesMaxId;

            if (handMRManager_.CenterTransform != null)
            {
                inputState_.position = HandCenterPosition - handMRManager_.CenterTransform.position;
                inputState_.gesturePointerPosition = fingers_[8].position - handMRManager_.CenterTransform.position;
            }
            else
            {
                inputState_.position = HandCenterPosition;
                inputState_.gesturePointerPosition = fingers_[8].position;
            }
            inputState_.position = Quaternion.Inverse(DefaultRotation) * inputState_.position;

            if (!handMRManager_.IsLockHandRotation)
            {
                if (inputState_.gestures == 1)
                {
                    if (ThisEitherHand == EitherHand.Left)
                    {
                        inputState_.rotation = transform.parent.rotation * handVRMain_.GetHandRotation(id);
                        inputState_.rotation = Quaternion.Inverse(DefaultRotation) * inputState_.rotation;
                        inputState_.rotation *= Quaternion.Euler(0f, BASE_ANGLE, 0f);
                    }
                    else
                    {
                        inputState_.rotation = transform.parent.rotation * handVRMain_.GetHandRotation(id);
                        inputState_.rotation = Quaternion.Inverse(DefaultRotation) * inputState_.rotation;
                        inputState_.rotation *= Quaternion.Euler(0f, -BASE_ANGLE, 0f);
                    }
                }
            }
            else
            {
                inputState_.rotation = transform.parent.rotation;
                inputState_.rotation = Quaternion.Inverse(DefaultRotation) * inputState_.rotation;
            }

            inputState_.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);

            InputSystem.QueueStateEvent(ThisEitherHand == EitherHand.Left ? XRController.leftHand : XRController.rightHand, inputState_);
#endif
        }

        void Update()
        {
            for (int loop = 0; loop < 5; loop++)
            {
                bool opened;

                bool fullTracking = true;
                for (int loop2 = 0; loop2 < 4; loop2++)
                {
                    if (fingers_[loop * 4 + loop2 + 1].GetComponent<HandVRPosition>().PhysicEnabled == false)
                    {
                        fullTracking = false;
                        break;
                    }
                }

                if (fullTracking)
                {
                    opened = calcFingerOpened(fingers_[loop * 4 + 2].position - fingers_[loop * 4 + 1].position,
                        fingers_[loop * 4 + 4].position - fingers_[loop * 4 + 3].position,
                        0.5f);
                }
                else
                {
                    opened = false;
                }

                fingerTracking_[loop] = fullTracking;
                fingerOpened_[loop] = opened;
            }

            if (IsTrackingHand)
            {
                int id = handVRMain_.GetIdFromHandednesses(ThisEitherHand);
                if (id >= 0)
                {
                    HandDirection = handVRMain_.GetHandDirection(id);
                }
            }

            inputUpdate();
        }

        public bool GetFingerTracking(int index)
        {
            return fingerTracking_[index];
        }

        public bool GetFingerOpened(int index)
        {
            return fingerOpened_[index];
        }

        public Transform GetFinger(int index)
        {
            return fingers_[index];
        }
    }
}
