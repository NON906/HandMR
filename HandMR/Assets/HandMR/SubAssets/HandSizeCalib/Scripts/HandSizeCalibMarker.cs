using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSizeCalibMarker : MonoBehaviour
{
    public Transform[] EdgePoints;

    void Start()
    {
        FindObjectOfType<HandSizeCalibMain>().MarkerTransforms = EdgePoints;
    }
}
