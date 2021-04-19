using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class HandVRSphereHand : MonoBehaviour
    {
        public enum EitherHand
        {
            Left,
            Right
        }

        public EitherHand ThisEitherHand = EitherHand.Left;

        Transform[] fingers_ = new Transform[21];
        bool[] fingerTracking_ = new bool[5];
        bool[] fingerOpened_ = new bool[5];
        HandVRMain handVRMain_;

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

        bool calcFingerOpened(Vector3 rootVec, Vector3 tipVec, float targetCos)
        {
            rootVec.Normalize();
            tipVec.Normalize();

            float cos = rootVec.x * tipVec.x + rootVec.y * tipVec.y + rootVec.z * tipVec.z;

            return cos > targetCos;
        }

        void Start()
        {
            handVRMain_ = FindObjectOfType<HandVRMain>();

            foreach (Transform child in transform)
            {
                HandVRPosition posObj = child.GetComponent<HandVRPosition>();
                if (posObj != null)
                {
                    fingers_[posObj.Index] = child;
                    posObj.ThisEitherHand = ThisEitherHand;
                }
            }
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
