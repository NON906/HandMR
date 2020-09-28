using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandSizeCalibMain : MonoBehaviour
{
    public Text ExplanationText;
    public Camera MainCamera;

    public Transform[] MarkerTransforms
    {
        get;
        set;
    } = null;

    bool captureButtonIsClicked_ = false;

#if UNITY_ANDROID
    [DllImport("hand3d")]
    static extern void handSizeCalibFindHomography(float x0, float y0,
        float x1, float y1,
        float x2, float y2,
        float x3, float y3,
        float size);
    [DllImport("hand3d")]
    static extern void handSizeCalibTransform(ref float x, ref float y);
#endif

#if UNITY_IOS
    [DllImport("__Internal")]
    static extern void handSizeCalibFindHomography(float x0, float y0,
        float x1, float y1,
        float x2, float y2,
        float x3, float y3,
        float size);
    [DllImport("__Internal")]
    static extern void handSizeCalibTransform(ref float x, ref float y);
#endif

    float calcLength(float[] pos, float[] basePos)
    {
        return Mathf.Sqrt((pos[0] - basePos[0]) * (pos[0] - basePos[0]) + (pos[1] - basePos[1]) * (pos[1] - basePos[1]));
    }

    IEnumerator Start()
    {
        ExplanationText.text = "マーカーを映してください";

        while (MarkerTransforms == null)
        {
            yield return null;
        }

        ExplanationText.text = "マーカーの近くの平面に手を置き、撮影ボタンを押してください";

        HandVRMain handVRMain = FindObjectOfType<HandVRMain>();

        for (; ; )
        {
            while (!captureButtonIsClicked_)
            {
                yield return null;
            }
            captureButtonIsClicked_ = false;

            float[][] landmarks = new float[21][];
            for (int index = 0; index < 21; index++)
            {
                landmarks[index] = handVRMain.GetLandmarkOnImage(0, index);
                if (landmarks[index] == null)
                {
                    break;
                }
            }
            if (landmarks[20] == null)
            {
                ExplanationText.text = "手の認識に失敗しました。もう一度撮影してください";
                continue;
            }

            Vector3 point0 = MainCamera.WorldToViewportPoint(MarkerTransforms[0].position);
            Vector3 point1 = MainCamera.WorldToViewportPoint(MarkerTransforms[1].position);
            Vector3 point2 = MainCamera.WorldToViewportPoint(MarkerTransforms[2].position);
            Vector3 point3 = MainCamera.WorldToViewportPoint(MarkerTransforms[3].position);

            handSizeCalibFindHomography(
                point0.x * Screen.width / Screen.height, 1f - point0.y,
                point1.x * Screen.width / Screen.height, 1f - point1.y,
                point2.x * Screen.width / Screen.height, 1f - point2.y,
                point3.x * Screen.width / Screen.height, 1f - point3.y,
                0.14f);

            for (int index = 0; index < 21; index++)
            {
                handSizeCalibTransform(ref landmarks[index][0], ref landmarks[index][1]);
            }

            float angle = -Mathf.Atan2(landmarks[9][1] - landmarks[0][1], landmarks[9][0] - landmarks[0][0]) + 90f * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
            int[] pointIds = new int[] { 0, 5, 9, 13, 17 };
            for (int loop = 0; loop < pointIds.Length; loop++)
            {
                float x = landmarks[pointIds[loop]][0] - landmarks[0][0];
                float y = landmarks[pointIds[loop]][1] - landmarks[0][1];
                PlayerPrefs.SetFloat("HandMR_PointPosX_" + loop, x * cos - y * sin);
                PlayerPrefs.SetFloat("HandMR_PointPosY_" + loop, x * sin + y * cos);
            }

            PlayerPrefs.SetFloat("HandMR_PointLength_0", calcLength(landmarks[1], landmarks[0]));
            PlayerPrefs.SetFloat("HandMR_PointLength_1", calcLength(landmarks[2], landmarks[1]));
            PlayerPrefs.SetFloat("HandMR_PointLength_2", calcLength(landmarks[3], landmarks[2]));
            PlayerPrefs.SetFloat("HandMR_PointLength_3", calcLength(landmarks[4], landmarks[3]));

            PlayerPrefs.SetFloat("HandMR_PointLength_4", calcLength(landmarks[6], landmarks[5]));
            PlayerPrefs.SetFloat("HandMR_PointLength_5", calcLength(landmarks[7], landmarks[6]));
            PlayerPrefs.SetFloat("HandMR_PointLength_6", calcLength(landmarks[8], landmarks[7]));

            PlayerPrefs.SetFloat("HandMR_PointLength_7", calcLength(landmarks[10], landmarks[9]));
            PlayerPrefs.SetFloat("HandMR_PointLength_8", calcLength(landmarks[11], landmarks[10]));
            PlayerPrefs.SetFloat("HandMR_PointLength_9", calcLength(landmarks[12], landmarks[11]));

            PlayerPrefs.SetFloat("HandMR_PointLength_10", calcLength(landmarks[14], landmarks[13]));
            PlayerPrefs.SetFloat("HandMR_PointLength_11", calcLength(landmarks[15], landmarks[14]));
            PlayerPrefs.SetFloat("HandMR_PointLength_12", calcLength(landmarks[16], landmarks[15]));

            PlayerPrefs.SetFloat("HandMR_PointLength_13", calcLength(landmarks[18], landmarks[17]));
            PlayerPrefs.SetFloat("HandMR_PointLength_14", calcLength(landmarks[19], landmarks[18]));
            PlayerPrefs.SetFloat("HandMR_PointLength_15", calcLength(landmarks[20], landmarks[19]));

            PlayerPrefs.Save();

            ExplanationText.text = "設定に成功しました";

            handVRMain.ResetHandValues();
        }
    }

    public void ClickCaptureButton()
    {
        captureButtonIsClicked_ = true;
    }

    public void ClickBackButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
