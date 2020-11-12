using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class ResetButton : MonoBehaviour, IControlObject
    {
        public Color FocusedColor = Color.yellow;
        public Color DetectColor = Color.red;
        public Transform[] Resetables;

        Material material_;
        Color defaultColor_;

        HashSet<HandVRSphereHand.EitherHand> focusStartHands_ = new HashSet<HandVRSphereHand.EitherHand>();
        HashSet<HandVRSphereHand.EitherHand> focusHands_ = new HashSet<HandVRSphereHand.EitherHand>();

        Vector3[] resetablesPosition_;
        Quaternion[] resetablesRotation_;

        void Start()
        {
            material_ = GetComponent<Renderer>().material;
            defaultColor_ = material_.color;

            resetablesPosition_ = new Vector3[Resetables.Length];
            resetablesRotation_ = new Quaternion[Resetables.Length];
            for (int loop = 0; loop < Resetables.Length; loop++)
            {
                resetablesPosition_[loop] = Resetables[loop].position;
                resetablesRotation_[loop] = Resetables[loop].rotation;
            }
        }

        public void StartFocus(HandVRSphereHand.EitherHand hand)
        {
            focusStartHands_.Add(hand);
            StartCoroutine(focusStartCoroutine(hand));
        }

        IEnumerator focusStartCoroutine(HandVRSphereHand.EitherHand hand)
        {
            yield return null;
            if (focusStartHands_.Remove(hand))
            {
                if (focusHands_.Count == 0 && material_.color != DetectColor)
                {
                    material_.color = FocusedColor;
                }
                focusHands_.Add(hand);
            }
        }

        public void EndFocus(HandVRSphereHand.EitherHand hand)
        {
            focusHands_.Remove(hand);
            focusStartHands_.Remove(hand);
            if (focusHands_.Count == 0)
            {
                material_.color = defaultColor_;
            }
        }

        void touch(HandVRSphereHand.EitherHand hand)
        {
            if (focusHands_.Contains(hand))
            {
                detect();

                focusHands_.Remove(hand);
                material_.color = DetectColor;
            }
        }

        public void StartTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {
            touch(hand);
        }

        public void StartGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {

        }

        public void StayGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {

        }

        public void EndGrab(HandVRSphereHand.EitherHand hand)
        {

        }

        public void StayTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {
            touch(hand);
        }

        public void EndTouch(HandVRSphereHand.EitherHand hand)
        {

        }

        void detect()
        {
            for (int loop = 0; loop < Resetables.Length; loop++)
            {
                Resetables[loop].position = resetablesPosition_[loop];
                Resetables[loop].rotation = resetablesRotation_[loop];
            }
        }
    }
}
