using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class HandSizeCalibMarker : MonoBehaviour
    {
        public Transform[] EdgePoints;
        public Material URPMaterial;

        void Start()
        {
#if ENABLE_URP
            GetComponentInChildren<Renderer>().material = URPMaterial;
#endif

            FindObjectOfType<HandSizeCalibMain>().MarkerTransforms = EdgePoints;
        }
    }
}
