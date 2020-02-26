using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

public class ARKitCameraTarget : MonoBehaviour
{
    // ARのカメラ
    public GameObject CameraObject;
    // カメラ画像
    public GameObject BackGround;

    // メインカメラ
    TrackingCameraPosition trackingCamera_;
    public TrackingCameraPosition TrackingCamera
    {
        get
        {
            return trackingCamera_;
        }
        set
        {
            trackingCamera_ = value;
#if UNITY_IOS
            trackingCamera_.GetComponent<Camera>().enabled = plane_ != null;
#endif
        }
    }


    // トラッキングの位置中心
    Vector3 poseCenter_ = Vector3.zero;
    Quaternion poseCenterRotation_ = Quaternion.identity;

#if UNITY_IOS
    // 認識した床
    ARPlaneAnchorGameObject plane_ = null;

    UnityARAnchorManager unityARAnchorManager_;

    void Start()
    {
        unityARAnchorManager_ = new UnityARAnchorManager();
        UnityARUtility.InitializePlanePrefab(new GameObject());
    }

    void OnDestroy()
    {
        unityARAnchorManager_.Destroy();
    }

    void Update()
    {
        if (plane_ == null || Input.touchCount > 0)
        {
            // 新しい床の取得
            plane_ = null;
            ARPlaneAnchorGameObject newPlane = null;
            float newPlaneDistance = float.PositiveInfinity;
            IEnumerable<ARPlaneAnchorGameObject> arpags = unityARAnchorManager_.GetCurrentPlaneAnchors();
            foreach (ARPlaneAnchorGameObject arpag in arpags)
            {
                float distance = Vector2.Distance(new Vector2(CameraObject.transform.position.x, CameraObject.transform.position.z), new Vector2(arpag.planeAnchor.center.x, arpag.planeAnchor.center.z));
                if (arpag.planeAnchor.alignment != ARPlaneAnchorAlignment.ARPlaneAnchorAlignmentVertical && newPlaneDistance >= distance)
                {
                    newPlane = arpag;
                    newPlaneDistance = distance;
                }
            }

            if (newPlane != null)
            {
                plane_ = newPlane;

                ResetPosition();

                // カメラ画像を非表示
                BackGround.SetActive(false);

                if (trackingCamera_ != null)
                {
                    trackingCamera_.GetComponent<Camera>().enabled = true;
                }
            }

            if (plane_ == null)
            {
                // カメラ画像を表示
                BackGround.SetActive(true);

                if (trackingCamera_ != null)
                {
                    trackingCamera_.GetComponent<Camera>().enabled = false;
                }
            }
        }

        // 移動
        transform.position = Quaternion.Inverse(poseCenterRotation_) * (CameraObject.transform.position - poseCenter_);
        transform.rotation = Quaternion.Inverse(poseCenterRotation_) * CameraObject.transform.rotation;
    }
#endif

    // 位置のリセット
    public void ResetPosition()
    {
#if UNITY_IOS
        if (CameraObject == null)
        {
            return;
        }

        poseCenter_ = CameraObject.transform.position;
        if (plane_ != null)
        {
            poseCenter_ += new Vector3(0f, plane_.gameObject.transform.position.y, 0f);
        }

        poseCenterRotation_ = Quaternion.Euler(0f, CameraObject.transform.eulerAngles.y, 0f);
#endif
    }
}
