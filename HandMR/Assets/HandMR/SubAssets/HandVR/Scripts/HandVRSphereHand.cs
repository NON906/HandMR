using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandVRSphereHand : MonoBehaviour
{
    public enum EitherHand
    {
        Left,
        Right
    }

    public int Id;

    Transform[] fingers_ = new Transform[21];
    bool[] fingerTracking_ = new bool[5];
    bool[] fingerOpened_ = new bool[5];

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

    public EitherHand ThisEitherHand
    {
        get;
        private set;
    } = EitherHand.Left;

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
        foreach (Transform child in transform)
        {
            HandVRPosition posObj = child.GetComponent<HandVRPosition>();
            if (posObj != null)
            {
                fingers_[posObj.Index] = child;
                posObj.Id = Id;
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

        HandDirection = Vector3.Cross(fingers_[5].position - fingers_[0].position, fingers_[17].position - fingers_[0].position).normalized;
        ThisEitherHand = EitherHand.Left;
        if (Camera.main != null && Vector3.Dot(HandDirection, Camera.main.transform.forward.normalized) < 0f)
        {
            HandDirection *= -1f;
            ThisEitherHand = EitherHand.Right;
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
