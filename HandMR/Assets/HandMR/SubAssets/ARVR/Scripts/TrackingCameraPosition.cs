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

        public CameraTarget Target;

        void Start()
        {
            rigidBody_ = GetComponent<Rigidbody>();
            collider_ = GetComponent<Collider>();
            collider_.enabled = false;
            XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), true);
        }

        void FixedUpdate()
        {
            if (!collider_.enabled && Target.IsTracking)
            {
                transform.localPosition = Target.transform.position;
                transform.localRotation = Target.transform.rotation;
                collider_.enabled = true;
            }
            else if (Target.IsTracking)
            {
                transform.localRotation = Target.transform.rotation;
                rigidBody_.AddForce((Target.transform.position - transform.localPosition) / Time.fixedDeltaTime - rigidBody_.velocity, ForceMode.VelocityChange);
            }
            else
            {
                collider_.enabled = false;
            }
        }
    }
}
