using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentMainCamera : MonoBehaviour
{
    public Transform MainCameraTransform = null;

    IEnumerator Start()
    {
        if (MainCameraTransform == null)
        {
            while (Camera.main == null)
            {
                yield return null;
            }
            MainCameraTransform = Camera.main.transform;
        }

        transform.parent = MainCameraTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
