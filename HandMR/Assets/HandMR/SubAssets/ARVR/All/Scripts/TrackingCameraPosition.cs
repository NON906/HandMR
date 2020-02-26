using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))]
public class TrackingCameraPosition : MonoBehaviour
{
    Rigidbody rigidBody_;

    GameObject target_ = null;
    public GameObject Target
    {
        get
        {
            return target_;
        }
        set
        {
            target_ = value;
            CameraTarget cameraTarget = target_.GetComponent<CameraTarget>();
            if (cameraTarget != null)
            {
                cameraTarget.TrackingCamera = this;
            }
            ARKitCameraTarget arKitCameraTarget = target_.GetComponent<ARKitCameraTarget>();
            if (arKitCameraTarget != null)
            {
                arKitCameraTarget.TrackingCamera = this;
            }
        }
    }

    void Start()
    {
        rigidBody_ = GetComponent<Rigidbody>();
        XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), true);
    }

    void FixedUpdate()
    {
        if (target_ != null)
        {
            if (rigidBody_ != null && !rigidBody_.isKinematic)
            {
                transform.localRotation = target_.transform.rotation;
                rigidBody_.AddForce((target_.transform.position - transform.localPosition) / Time.fixedDeltaTime - rigidBody_.velocity, ForceMode.VelocityChange);
            }
            else
            {
                transform.localRotation = target_.transform.rotation;
                transform.localPosition = target_.transform.position;
            }
        }
    }
}
