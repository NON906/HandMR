using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace HandMR
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class TrackingCameraPosition : MonoBehaviour
    {
        Rigidbody rigidBody_;
        Collider collider_;
        HandMRManager handMRManager_;

        public CameraTarget Target;

        void Start()
        {
            rigidBody_ = GetComponent<Rigidbody>();
            collider_ = GetComponent<Collider>();
            collider_.enabled = false;
            XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), true);
            handMRManager_ = GetComponentInParent<HandMRManager>();
        }

        void FixedUpdate()
        {
            if (!collider_.enabled && Target.IsTracking)
            {
                Vector3 targetPosition = Target.transform.position;
                if (handMRManager_.FreezePositionX)
                {
                    targetPosition.x = transform.localPosition.x;
                }
                if (handMRManager_.FreezePositionY)
                {
                    targetPosition.y = transform.localPosition.y;
                }
                if (handMRManager_.FreezePositionZ)
                {
                    targetPosition.z = transform.localPosition.z;
                }
                transform.localPosition = targetPosition;
                transform.localRotation = Target.transform.rotation;
                collider_.enabled = true;
            }
            else if (Target.IsTracking)
            {
                transform.localRotation = Target.transform.rotation;
                Vector3 targetForce = (Target.transform.position - transform.localPosition) / Time.fixedDeltaTime;
                if (handMRManager_.FreezePositionX)
                {
                    targetForce.x = 0f;
                }
                if (handMRManager_.FreezePositionY)
                {
                    targetForce.y = 0f;
                }
                if (handMRManager_.FreezePositionZ)
                {
                    targetForce.z = 0f;
                }
                rigidBody_.AddForce(targetForce - rigidBody_.velocity, ForceMode.VelocityChange);
            }
            else
            {
                collider_.enabled = false;
            }
        }
    }
}
