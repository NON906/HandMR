using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class SetParentMainCamera : MonoBehaviour
    {
        Transform mainCameraTransform_ = null;
        public Transform MainCameraTransform
        {
            get
            {
                return mainCameraTransform_;
            }
            set
            {
                mainCameraTransform_ = value;
                transform.parent = MainCameraTransform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
