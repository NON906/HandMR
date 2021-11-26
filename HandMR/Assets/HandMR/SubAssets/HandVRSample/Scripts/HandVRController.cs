using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class HandVRController : MonoBehaviour
    {
        public float TouchSize = 0.25f;
        public float PickSize = 0.05f;
        public Transform MainCameraTransform = null;

        HandVRSphereHand[] sphereHands_;

        IControlObject[] focusedObjects_ = new IControlObject[2];
        bool[] isGrab_ = new bool[2];
        bool[] isTouch_ = new bool[2];

        Coroutine[] startGrabCoroutineRunning_ = new Coroutine[2];
        Coroutine[] endGrabCoroutineRunning_ = new Coroutine[2];

        IEnumerator Start()
        {
            sphereHands_ = FindObjectsOfType<HandVRSphereHand>();

            while (MainCameraTransform == null)
            {
                MainCameraTransform = sphereHands_[0].GetComponent<SetParentMainCamera>().MainCameraTransform;
                yield return null;
            }
        }

        void Update()
        {
            if (MainCameraTransform == null)
            {
                return;
            }

            bool[] isDetected = new bool[2];

            foreach (HandVRSphereHand sphereHand in sphereHands_)
            {
                if (!sphereHand.IsTrackingHand)
                {
                    continue;
                }

                if (isDetected[(int)sphereHand.ThisEitherHand])
                {
                    continue;
                }
                isDetected[(int)sphereHand.ThisEitherHand] = true;

                int layerMask = LayerMask.GetMask("ControlObject");

                RaycastHit hit;
                IControlObject newObject = null;
                bool rayIsHit = Physics.Raycast(sphereHand.GetFinger(8).position - (sphereHand.GetFinger(8).position - MainCameraTransform.position).normalized * 2f,
                    (sphereHand.GetFinger(8).position - MainCameraTransform.position).normalized,
                    out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide);
                if (rayIsHit)
                {
                    newObject = hit.transform.GetComponent<IControlObject>();
                }

                int handId = (int)sphereHand.ThisEitherHand;

                if (focusedObjects_[handId] != newObject && focusedObjects_[handId] != null && !isGrab_[handId])
                {
                    focusedObjects_[handId].EndFocus(sphereHand.ThisEitherHand);
                    focusedObjects_[handId] = null;
                }

                if (focusedObjects_[handId] == null && newObject != null)
                {
                    newObject.StartFocus(sphereHand.ThisEitherHand);
                    focusedObjects_[handId] = newObject;
                }

                if (focusedObjects_[handId] != null)
                {
                    Collider col = null;
                    if (rayIsHit && hit.distance < 2f + TouchSize)
                    {
                        col = hit.collider;
                    }

                    if (!isTouch_[handId])
                    {
                        if (col != null && col.GetComponent<IControlObject>() == focusedObjects_[handId])
                        {
                            focusedObjects_[handId].StartTouch(sphereHand.ThisEitherHand, sphereHand.transform.TransformPoint(sphereHand.GetFinger(8).position));
                            isTouch_[handId] = true;
                        }
                    }
                    else
                    {
                        isTouch_[handId] = false;

                        if (col != null && col.GetComponent<IControlObject>() == focusedObjects_[handId])
                        {
                            focusedObjects_[handId].StayTouch(sphereHand.ThisEitherHand, sphereHand.transform.TransformPoint(sphereHand.GetFinger(8).position));
                            isTouch_[handId] = true;
                        }
                        if (!isTouch_[handId])
                        {
                            focusedObjects_[handId].EndTouch(sphereHand.ThisEitherHand);
                        }
                    }

                    bool grabCountrol = true;
                    for (int loop = 0; loop < 2; loop++)
                    {
                        if (handId != loop && focusedObjects_[handId] == focusedObjects_[loop] && isGrab_[loop])
                        {
                            grabCountrol = false;
                        }
                    }

                    if (grabCountrol)
                    {
                        bool grabed = true;
                        bool opened = true;
#if false
                        for (int loop = 1; loop < 5; loop++)
                        {
                            if (sphereHand.GetFingerOpened(loop))
                            {
                                grabed = false;
                            }
                            else
                            {
                                opened = false;
                            }
                        }
                        if (Vector3.Distance(sphereHand.GetFinger(4).position, sphereHand.GetFinger(8).position) < PickSize)
                        {
                            grabed = true;
                            opened = false;
                        }
#endif
                        grabed = sphereHand.IsGrab;
                        opened = !grabed;

                        if (!isGrab_[handId] && grabed)
                        {
                            startGrabCoroutineRunning_[handId] = StartCoroutine(startGrabCoroutine(sphereHand.ThisEitherHand, sphereHand.transform.TransformPoint(sphereHand.GetFinger(8).position)));
                            isGrab_[handId] = true;
                        }
                        else if (isGrab_[handId] && grabed)
                        {
                            if (startGrabCoroutineRunning_[handId] == null)
                            {
                                focusedObjects_[handId].StayGrab(sphereHand.ThisEitherHand, sphereHand.transform.TransformPoint(sphereHand.GetFinger(8).position));
                            }
                            if (endGrabCoroutineRunning_[handId] != null)
                            {
                                StopCoroutine(endGrabCoroutineRunning_[handId]);
                                endGrabCoroutineRunning_[handId] = null;
                            }
                        }
                        else if (isGrab_[handId] && opened)
                        {
                            if (startGrabCoroutineRunning_[handId] != null)
                            {
                                StopCoroutine(startGrabCoroutineRunning_[handId]);
                                startGrabCoroutineRunning_[handId] = null;
                            }
                            else if (endGrabCoroutineRunning_[handId] == null)
                            {
                                endGrabCoroutineRunning_[handId] = StartCoroutine(endGrabCoroutine(sphereHand.ThisEitherHand));
                            }
                        }
                    }
                }
            }

            for (int loop = 0; loop < 2; loop++)
            {
                if (!isDetected[loop] && focusedObjects_[loop] != null && !isGrab_[loop])
                {
                    focusedObjects_[loop].EndFocus((HandVRSphereHand.EitherHand)loop);
                    focusedObjects_[loop] = null;
                }
            }
        }

        IEnumerator startGrabCoroutine(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {
            yield return new WaitForSeconds(0.1f);

            focusedObjects_[(int)hand].StartGrab(hand, centerPosition);
            startGrabCoroutineRunning_[(int)hand] = null;
        }

        IEnumerator endGrabCoroutine(HandVRSphereHand.EitherHand hand)
        {
            yield return new WaitForSeconds(0.1f);

            focusedObjects_[(int)hand].EndGrab(hand);
            endGrabCoroutineRunning_[(int)hand] = null;
            isGrab_[(int)hand] = false;
        }
    }
}
