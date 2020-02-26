using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlObject
{
    void StartFocus(HandVRSphereHand.EitherHand hand);
    void EndFocus(HandVRSphereHand.EitherHand hand);
    void StartGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
    void StayGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition);
    void EndGrab(HandVRSphereHand.EitherHand hand);
}
