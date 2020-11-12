using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class CanCarryObject : MonoBehaviour, IControlObject
    {
        public Color FocusedColor = Color.yellow;
        public Color GrabColor = Color.red;

        public Transform MainCameraTransform = null;

        Material material_;
        Color defaultColor_;
        Rigidbody rigidbody_;

        HashSet<HandVRSphereHand.EitherHand> focusHands_ = new HashSet<HandVRSphereHand.EitherHand>();
        HashSet<HandVRSphereHand.EitherHand> grabHands_ = new HashSet<HandVRSphereHand.EitherHand>();

        Transform targetTransformParent_;
        Transform targetTransform_;

        void Start()
        {
            material_ = GetComponent<Renderer>().material;
            defaultColor_ = material_.color;
            rigidbody_ = GetComponent<Rigidbody>();

            targetTransformParent_ = new GameObject().transform;
            targetTransform_ = new GameObject().transform;
            if (MainCameraTransform != null)
            {
                targetTransformParent_.parent = MainCameraTransform;
            }
            else
            {
                targetTransformParent_.parent = Camera.main.transform;
            }
            targetTransform_.parent = targetTransformParent_;
        }

        public void StartFocus(HandVRSphereHand.EitherHand hand)
        {
            if (focusHands_.Count == 0)
            {
                material_.color = FocusedColor;
            }
            focusHands_.Add(hand);
        }

        public void EndFocus(HandVRSphereHand.EitherHand hand)
        {
            focusHands_.Remove(hand);
            if (focusHands_.Count == 0)
            {
                material_.color = defaultColor_;
            }
        }

        public void StartGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {
            if (grabHands_.Count == 0)
            {
                material_.color = GrabColor;

                targetTransformParent_.position = centerPosition;
                targetTransformParent_.LookAt(transform);
                targetTransform_.rotation = transform.rotation;
                targetTransform_.position = transform.position;
            }
            grabHands_.Add(hand);
        }

        public void StayGrab(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {
            targetTransformParent_.position = centerPosition;
        }

        public void EndGrab(HandVRSphereHand.EitherHand hand)
        {
            grabHands_.Remove(hand);
            if (grabHands_.Count == 0)
            {
                if (focusHands_.Count > 0)
                {
                    material_.color = FocusedColor;
                }
                else
                {
                    material_.color = defaultColor_;
                }
            }
        }

        public void StartTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {

        }

        public void StayTouch(HandVRSphereHand.EitherHand hand, Vector3 centerPosition)
        {

        }

        public void EndTouch(HandVRSphereHand.EitherHand hand)
        {

        }

        void FixedUpdate()
        {
            if (grabHands_.Count > 0)
            {
                rigidbody_.useGravity = false;

                float angle1 = Vector3.Angle(transform.forward, targetTransform_.forward);
                Vector3 axis1 = Vector3.Cross(transform.forward, targetTransform_.forward).normalized;
                Quaternion quat1 = Quaternion.AngleAxis(angle1, axis1);
                Vector3 right = quat1 * transform.right;
                Vector3 rightTarget = quat1 * targetTransform_.right;
                float angle2 = Vector3.Angle(right, rightTarget);
                Vector3 axis2 = Vector3.Cross(right, rightTarget).normalized;
                rigidbody_.AddTorque((angle1 * axis1 + angle2 * axis2) * Mathf.Deg2Rad / Time.fixedDeltaTime - rigidbody_.angularVelocity, ForceMode.VelocityChange);

                rigidbody_.AddForce((targetTransform_.position - transform.position) / Time.fixedDeltaTime - rigidbody_.velocity, ForceMode.VelocityChange);
            }
            else
            {
                rigidbody_.useGravity = true;
            }
        }

        void OnDestroy()
        {
            if (targetTransform_ != null)
            {
                Destroy(targetTransform_.gameObject);
            }
            if (targetTransformParent_ != null)
            {
                Destroy(targetTransformParent_.gameObject);
            }
        }
    }
}