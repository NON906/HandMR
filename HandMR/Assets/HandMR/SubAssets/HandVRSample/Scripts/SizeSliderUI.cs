using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class SizeSliderUI : MonoBehaviour
    {
        public Transform TargetTransform;

        Vector3 defaultScale_;

        void Start()
        {
            defaultScale_ = TargetTransform.localScale;
        }

        public void ValueChange(float val)
        {
            TargetTransform.localScale = defaultScale_ * val;
        }
    }
}
