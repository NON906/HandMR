using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    [RequireComponent(typeof(LineRenderer))]
    public class MRHandFingerLine : MonoBehaviour
    {
        public int[] Ids = new int[5];

        HandVRSphereHand sphereHand_;
        LineRenderer lineRenderer_;

        void Start()
        {
            sphereHand_ = GetComponentInParent<HandVRSphereHand>();
            lineRenderer_ = GetComponent<LineRenderer>();
            lineRenderer_.useWorldSpace = false;
            lineRenderer_.enabled = false;
        }

        void LateUpdate()
        {
            if (sphereHand_.IsTrackingHand)
            {
                Vector3[] positions = new Vector3[Ids.Length];
                for (int loop = 0; loop < Ids.Length; loop++)
                {
                    positions[loop] = sphereHand_.GetFinger(Ids[loop]).localPosition;
                }
                lineRenderer_.SetPositions(positions);
                lineRenderer_.enabled = true;
            }
            else
            {
                lineRenderer_.enabled = false;
            }
        }
    }
}
