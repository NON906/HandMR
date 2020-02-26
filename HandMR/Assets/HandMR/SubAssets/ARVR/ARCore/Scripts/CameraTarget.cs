using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID && DOWNLOADED_ARCORE
using GoogleARCore;
#endif
#if DOWNLOADED_ARCORE
using UnityEngine.SpatialTracking;
#endif

public class CameraTarget : MonoBehaviour
{
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
#if UNITY_ANDROID && DOWNLOADED_HOLOGLA
            trackingCamera_.GetComponent<Camera>().enabled = plane_ != null;
#endif
        }
    }

    // トラッキングの位置中心
    Vector3 poseCenter_ = Vector3.zero;
    Quaternion poseCenterRotation_ = Quaternion.identity;

#if UNITY_ANDROID && DOWNLOADED_HOLOGLA
    // 認識した床
    DetectedPlane plane_ = null;
    // トラッキングのスクリプト
    TrackedPoseDriver poseDriver_;

    void Start()
    {
        poseDriver_ = FindObjectOfType<TrackedPoseDriver>();
    }

    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        if (plane_ == null || plane_.TrackingState == TrackingState.Stopped || Input.touchCount > 0)
        {
            // 新しい床の取得
            plane_ = null;
            DetectedPlane newPlane = null;
            float newPlaneDistance = float.PositiveInfinity;
            List<DetectedPlane> planes = new List<DetectedPlane>();
            Session.GetTrackables<DetectedPlane>(planes, TrackableQueryFilter.All);
            foreach (DetectedPlane plane in planes)
            {
                float distance = Vector2.Distance(new Vector2(poseDriver_.transform.position.x, poseDriver_.transform.position.z), new Vector2(plane.CenterPose.position.x, plane.CenterPose.position.z));
                if (plane.PlaneType != DetectedPlaneType.Vertical && newPlaneDistance >= distance)
                {
                    newPlane = plane;
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
        transform.position = Quaternion.Inverse(poseCenterRotation_) * (poseDriver_.transform.position - poseCenter_);
        transform.rotation = Quaternion.Inverse(poseCenterRotation_) * poseDriver_.transform.rotation;
    }
#endif

    // 位置のリセット
    public void ResetPosition()
    {
#if UNITY_ANDROID && DOWNLOADED_ARCORE
        poseCenter_ = poseDriver_.transform.position;
        if (plane_ != null)
        {
            poseCenter_ += new Vector3(0f, plane_.CenterPose.position.y, 0f);
        }

        poseCenterRotation_ = Quaternion.Euler(0f, poseDriver_.transform.eulerAngles.y, 0f);
#endif
    }
}
