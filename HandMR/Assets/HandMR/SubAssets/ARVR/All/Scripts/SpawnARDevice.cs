using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnARDevice : MonoBehaviour
{
    public GameObject ARCorePrefab;
    public GameObject ARKitPrefab;

    void Start()
    {
        GameObject obj;

        TrackingCameraPosition trackingCameraPosition = FindObjectOfType<TrackingCameraPosition>();

#if UNITY_ANDROID
        obj = Instantiate(ARCorePrefab);
        trackingCameraPosition.Target = obj.GetComponentInChildren<CameraTarget>().gameObject;
#endif
#if UNITY_IOS
        obj = Instantiate(ARKitPrefab);
        trackingCameraPosition.Target = obj.GetComponentInChildren<ARKitCameraTarget>().gameObject;
#endif
    }
}
