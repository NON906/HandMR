using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HandMR
{
    public class HandVRMain : MonoBehaviour
    {
        const int RESIZE_HEIGHT = 512;
        const float DISABLE_TIME = 0.1f;
        const float START_DELAY_TIME = 0f;

        public RenderTexture InputRenderTexture;
        public Shader FlipShader;
        public float ShiftX = 0f;
        public float ShiftY = 0f;
        //public float HandSize = 0.13f;
        public float FieldOfView = 60f;
        public float IntervalTime = 0.04f;

        float[,][] landmarkArray_ = new float[2, 21][];
        double focalLength_;
        float[] fingerLengthArray_ = new float[16];
        Quaternion[] rotationQuaternions_ = new Quaternion[2];
        float lastEnableTime_ = 0f;
        Material filpMaterial_;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject multiHandMain_;

        [DllImport("hand3d")]
        static extern void hand3dInit([In]string filePath);
        [DllImport("hand3d")]
        static extern void hand3dGetCameraValues(out double fx, out double fy, out double cx, out double cy);
        [DllImport("hand3d")]
        static extern void hand3dInitWithValues(double fx, double fy, double cx, double cy);
        [DllImport("hand3d")]
        static extern void hand3dExec(out double rx, out double ry, out double rz, out double x, out double y, out double z,
            double x0, double y0,
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4);
        [DllImport("hand3d")]
        static extern void hand3dGet3dPosition(out double x, out double y, out double z, int pointId);
        [DllImport("hand3d")]
        static extern void hand3dSetHandPoint(int pointId, float x, float y, float z);
        [DllImport("hand3d")]
        static extern void hand3dReset();
#elif UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void multiHandCleanup();
        [DllImport("__Internal")]
        static extern void multiHandSetup(IntPtr graphName, int width, int height);
        [DllImport("__Internal")]
        static extern void multiHandSetFrame(IntPtr frameSource, int frameSourceSize);
        [DllImport("__Internal")]
        static extern void multiHandStartRunningGraph();
        [DllImport("__Internal")]
        static extern int multiHandGetHandCount();
        [DllImport("__Internal")]
        static extern float multiHandGetLandmark(int id, int index, int axis);
        [DllImport("__Internal")]
        static extern bool multiHandGetIsUpdated();
        [DllImport("__Internal")]
        static extern int multiHandGetHandednesses(int id);

        [DllImport("__Internal")]
        static extern void hand3dInit([In]string filePath);
        [DllImport("__Internal")]
        static extern void hand3dGetCameraValues(out double fx, out double fy, out double cx, out double cy);
        [DllImport("__Internal")]
        static extern void hand3dInitWithValues(double fx, double fy, double cx, double cy);
        [DllImport("__Internal")]
        static extern void hand3dExec(out double rx, out double ry, out double rz, out double x, out double y, out double z,
            double x0, double y0,
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4);
        [DllImport("__Internal")]
        static extern void hand3dGet3dPosition(out double x, out double y, out double z, int pointId);
        [DllImport("__Internal")]
        static extern void hand3dSetHandPoint(int pointId, float x, float y, float z);
        [DllImport("__Internal")]
        static extern void hand3dReset();
#else
        static void hand3dInit([In] string filePath) { }
        static void hand3dGetCameraValues(out double fx, out double fy, out double cx, out double cy)
        {
            fx = fy = cx = cy = 0.0;
        }
        static void hand3dInitWithValues(double fx, double fy, double cx, double cy) { }
        static void hand3dExec(out double rx, out double ry, out double rz, out double x, out double y, out double z,
            double x0, double y0,
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4)
        {
            rx = ry = rz = x = y = z = 0.0;
        }
        static void hand3dGet3dPosition(out double x, out double y, out double z, int pointId)
        {
            x = y = z = 0.0;
        }
        static void hand3dSetHandPoint(int pointId, float x, float y, float z) { }
        static void hand3dReset() { }
#endif

        Texture2D texture2D_;
        public Texture2D Texture2DRaw
        {
            get
            {
                return texture2D_;
            }
        }

        bool isStart_ = false;
        float nextUpdateFrameTime_ = float.PositiveInfinity;

        void Start()
        {
            int width = RESIZE_HEIGHT * Screen.width / Screen.height;

#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentUnityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                multiHandMain_ = new AndroidJavaObject("online.mumeigames.mediapipe.apps.multihandtrackinggpu.MultiHandMain", currentUnityActivity, width, RESIZE_HEIGHT);
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            texture2D_ = new Texture2D(width, RESIZE_HEIGHT, TextureFormat.BGRA32, false, false);
#else
            texture2D_ = new Texture2D(width, RESIZE_HEIGHT, TextureFormat.ARGB32, false, false);
#endif

            filpMaterial_ = new Material(FlipShader);

#if UNITY_IOS && !UNITY_EDITOR
            IntPtr graphName = Marshal.StringToHGlobalAnsi("handcputogpu");
            multiHandSetup(graphName, texture2D_.width, texture2D_.height);
            Marshal.FreeHGlobal(graphName);
#endif

            focalLength_ = 0.5 / Mathf.Tan(FieldOfView * Mathf.Deg2Rad * 0.5f);
            hand3dInitWithValues(focalLength_, focalLength_, 0.5, 0.5);

            resetHandValues();
        }

        void resetHandValues()
        {
            if (PlayerPrefs.HasKey("HandMR_PointLength_15"))
            {
                for (int loop = 0; loop < 5; loop++)
                {
                    hand3dSetHandPoint(loop, PlayerPrefs.GetFloat("HandMR_PointPosX_" + loop), PlayerPrefs.GetFloat("HandMR_PointPosY_" + loop), 0f);
                }

                for (int loop = 0; loop < fingerLengthArray_.Length; loop++)
                {
                    fingerLengthArray_[loop] = PlayerPrefs.GetFloat("HandMR_PointLength_" + loop);
                }
            }
            else
            {
                fingerLengthArray_ = new float[]
                {
                0.03f, 0.03f, 0.02f, 0.015f, 0.035f, 0.02f, 0.015f, 0.035f, 0.025f, 0.015f,
                0.035f, 0.025f, 0.015f, 0.03f, 0.015f, 0.015f
                };
            }
        }

        public void ResetHandValues()
        {
            hand3dReset();
            resetHandValues();
        }

        void Update()
        {
            updateFrame();
        }

        void updateFrame()
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!isStart_)
            {
                multiHandMain_.Call("startRunningGraph");
                nextUpdateFrameTime_ = Time.time + START_DELAY_TIME;
                isStart_ = true;
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            if (!isStart_)
            {
                multiHandStartRunningGraph();
                nextUpdateFrameTime_ = Time.time + START_DELAY_TIME;
                isStart_ = true;
            }
#endif

            if (nextUpdateFrameTime_ > Time.time)
            {
                return;
            }
            nextUpdateFrameTime_ += IntervalTime;

            RenderTexture reshapedTexture = RenderTexture.GetTemporary(texture2D_.width, texture2D_.height);
            if (filpMaterial_ != null)
            {
                Graphics.Blit(InputRenderTexture, reshapedTexture, filpMaterial_);
            }
            else
            {
                Graphics.Blit(InputRenderTexture, reshapedTexture);
            }

            RenderTexture.active = reshapedTexture;
            texture2D_.ReadPixels(new Rect(0, 0, texture2D_.width, texture2D_.height), 0, 0, false);
            texture2D_.Apply();
            RenderTexture.active = null;

            RenderTexture.ReleaseTemporary(reshapedTexture);

            byte[] frameImage = texture2D_.GetRawTextureData();

#if UNITY_ANDROID && !UNITY_EDITOR
            sbyte[] frameImageSigned = Array.ConvertAll(frameImage, b => unchecked((sbyte)b));
            multiHandMain_.Call("setFrame", frameImageSigned);
#endif

#if UNITY_IOS && !UNITY_EDITOR
            IntPtr frameIntPtr = Marshal.AllocHGlobal(frameImage.Length * Marshal.SizeOf<byte>());
            Marshal.Copy(frameImage, 0, frameIntPtr, frameImage.Length);
            multiHandSetFrame(frameIntPtr, frameImage.Length);
            Marshal.FreeHGlobal(frameIntPtr);
#endif
        }

        float[] calcLandmarkPoint(float[] target, float[] parent, float[] parentReal, float length, float mulSize)
        {
            float baseX = target[0] - parent[0];
            float baseY = target[1] - parent[1];
            int zSign = target[2] - parent[2] > 0f ? 1 : -1;
            float[] ret = new float[] {
            mulSize * baseX + parentReal[0],
            mulSize * baseY + parentReal[1],
            Mathf.Sqrt(length * length - mulSize * mulSize * baseX * baseX - mulSize * mulSize * baseY * baseY) * zSign + parentReal[2],
        };
            if (float.IsNaN(ret[2]))
            {
                ret[2] = parentReal[2];
            }
            return ret;
        }

        float calcLength(float[] pos0, float[] pos1)
        {
            return Mathf.Sqrt((pos0[0] - pos1[0]) * (pos0[0] - pos1[0]) + (pos0[1] - pos1[1]) * (pos0[1] - pos1[1]));
        }

        public float[] GetLandmarkOnImage(int id, int index)
        {
            float[] ret;

#if UNITY_ANDROID && !UNITY_EDITOR
            ret = multiHandMain_.Call<float[]>("getLandmark", id, index);
#elif UNITY_IOS && !UNITY_EDITOR
            if (multiHandGetHandCount() <= id)
            {
                return null;
            }
            ret = new float[3];
            for (int loop = 0; loop < 3; loop++)
            {
                ret[loop] = multiHandGetLandmark(id, index, loop);
            }
#else
            return null;
#endif

            if (ret == null)
            {
                return null;
            }

            ret[0] = (ret[0] - 0.5f) * Screen.width / Screen.height + 0.5f;

            return ret;
        }

        Quaternion rotationQuaternion(float x, float y, float z)
        {
            float angle = Mathf.Sqrt(x * x + y * y + z * z);

            float qx, qy, qz, qw;
            if (angle > 0.0)
            {
                qx = x * Mathf.Sin(angle / 2f) / angle;
                qy = y * Mathf.Sin(angle / 2f) / angle;
                qz = z * Mathf.Sin(angle / 2f) / angle;
                qw = Mathf.Cos(angle / 2f);
            }
            else
            {
                qx = qy = qz = 0f;
                qw = 1f;
            }

            return new Quaternion(qx, qy, qz, qw);
        }

        void calcLandmark()
        {
            for (int id = 0; id < 2; id++)
            {
                float[][] landmarkArrayRaw = new float[21][];

                for (int index = 0; index < 21; index++)
                {
                    landmarkArrayRaw[index] = GetLandmarkOnImage(id, index);
                    if (landmarkArrayRaw[index] == null)
                    {
                        break;
                    }
                }

                if (landmarkArrayRaw[20] == null)
                {
                    for (int index = 0; index < 21; index++)
                    {
                        landmarkArray_[id, index] = null;
                    }
                    continue;
                }

                double rx, ry, rz, x, y, z;
                hand3dExec(out rx, out ry, out rz, out x, out y, out z,
                    landmarkArrayRaw[0][0], landmarkArrayRaw[0][1],
                    landmarkArrayRaw[5][0], landmarkArrayRaw[5][1],
                    landmarkArrayRaw[9][0], landmarkArrayRaw[9][1],
                    landmarkArrayRaw[13][0], landmarkArrayRaw[13][1],
                    landmarkArrayRaw[17][0], landmarkArrayRaw[17][1]);

                if (x * x + y * y + z * z > 0.75 * 0.75)
                {
                    for (int index = 0; index < 21; index++)
                    {
                        landmarkArray_[id, index] = null;
                    }
                    continue;
                }

                rotationQuaternions_[id] = Quaternion.Euler(0f, 0f, 180f) * rotationQuaternion((float)rx, (float)ry, (float)rz);

                landmarkArray_[id, 0] = new float[] { (float)x, (float)y, (float)z };

                hand3dGet3dPosition(out x, out y, out z, 1);
                landmarkArray_[id, 5] = new float[] { (float)x, (float)y, (float)z };
                hand3dGet3dPosition(out x, out y, out z, 2);
                landmarkArray_[id, 9] = new float[] { (float)x, (float)y, (float)z };
                hand3dGet3dPosition(out x, out y, out z, 3);
                landmarkArray_[id, 13] = new float[] { (float)x, (float)y, (float)z };
                hand3dGet3dPosition(out x, out y, out z, 4);
                landmarkArray_[id, 17] = new float[] { (float)x, (float)y, (float)z };

                float mulSize = (calcLength(landmarkArray_[id, 0], landmarkArray_[id, 5]) / calcLength(landmarkArrayRaw[0], landmarkArrayRaw[5])
                    + calcLength(landmarkArray_[id, 0], landmarkArray_[id, 9]) / calcLength(landmarkArrayRaw[0], landmarkArrayRaw[9])
                    + calcLength(landmarkArray_[id, 0], landmarkArray_[id, 13]) / calcLength(landmarkArrayRaw[0], landmarkArrayRaw[13])
                    + calcLength(landmarkArray_[id, 0], landmarkArray_[id, 17]) / calcLength(landmarkArrayRaw[0], landmarkArrayRaw[17]))
                    / 4f;

                landmarkArray_[id, 1] = calcLandmarkPoint(landmarkArrayRaw[1], landmarkArrayRaw[0], landmarkArray_[id, 0],
                    fingerLengthArray_[0], mulSize);
                landmarkArray_[id, 2] = calcLandmarkPoint(landmarkArrayRaw[2], landmarkArrayRaw[1], landmarkArray_[id, 1],
                    fingerLengthArray_[1], mulSize);
                landmarkArray_[id, 3] = calcLandmarkPoint(landmarkArrayRaw[3], landmarkArrayRaw[2], landmarkArray_[id, 2],
                    fingerLengthArray_[2], mulSize);
                landmarkArray_[id, 4] = calcLandmarkPoint(landmarkArrayRaw[4], landmarkArrayRaw[3], landmarkArray_[id, 3],
                    fingerLengthArray_[3], mulSize);

                landmarkArray_[id, 6] = calcLandmarkPoint(landmarkArrayRaw[6], landmarkArrayRaw[5], landmarkArray_[id, 5],
                    fingerLengthArray_[4], mulSize);
                landmarkArray_[id, 7] = calcLandmarkPoint(landmarkArrayRaw[7], landmarkArrayRaw[6], landmarkArray_[id, 6],
                    fingerLengthArray_[5], mulSize);
                landmarkArray_[id, 8] = calcLandmarkPoint(landmarkArrayRaw[8], landmarkArrayRaw[7], landmarkArray_[id, 7],
                    fingerLengthArray_[6], mulSize);

                landmarkArray_[id, 10] = calcLandmarkPoint(landmarkArrayRaw[10], landmarkArrayRaw[9], landmarkArray_[id, 9],
                    fingerLengthArray_[7], mulSize);
                landmarkArray_[id, 11] = calcLandmarkPoint(landmarkArrayRaw[11], landmarkArrayRaw[10], landmarkArray_[id, 10],
                    fingerLengthArray_[8], mulSize);
                landmarkArray_[id, 12] = calcLandmarkPoint(landmarkArrayRaw[12], landmarkArrayRaw[11], landmarkArray_[id, 11],
                    fingerLengthArray_[9], mulSize);

                landmarkArray_[id, 14] = calcLandmarkPoint(landmarkArrayRaw[14], landmarkArrayRaw[13], landmarkArray_[id, 13],
                    fingerLengthArray_[10], mulSize);
                landmarkArray_[id, 15] = calcLandmarkPoint(landmarkArrayRaw[15], landmarkArrayRaw[14], landmarkArray_[id, 14],
                    fingerLengthArray_[11], mulSize);
                landmarkArray_[id, 16] = calcLandmarkPoint(landmarkArrayRaw[16], landmarkArrayRaw[15], landmarkArray_[id, 15],
                    fingerLengthArray_[12], mulSize);

                landmarkArray_[id, 18] = calcLandmarkPoint(landmarkArrayRaw[18], landmarkArrayRaw[17], landmarkArray_[id, 17],
                    fingerLengthArray_[13], mulSize);
                landmarkArray_[id, 19] = calcLandmarkPoint(landmarkArrayRaw[19], landmarkArrayRaw[18], landmarkArray_[id, 18],
                    fingerLengthArray_[14], mulSize);
                landmarkArray_[id, 20] = calcLandmarkPoint(landmarkArrayRaw[20], landmarkArrayRaw[19], landmarkArray_[id, 19],
                    fingerLengthArray_[15], mulSize);

                for (int index = 0; index < 21; index++)
                {
                    landmarkArray_[id, index][0] = landmarkArray_[id, index][0] + ShiftX;
                    landmarkArray_[id, index][1] = (landmarkArray_[id, index][1] + ShiftY) * -1f;
                    landmarkArray_[id, index][2] += (float)(focalLength_ * mulSize);
                }
            }
        }

        public float[] GetLandmark(int id, int index)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (multiHandMain_.Call<bool>("getIsUpdated"))
            {
                calcLandmark();
                lastEnableTime_ = Time.time;
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            if (multiHandGetIsUpdated())
            {
                calcLandmark();
                lastEnableTime_ = Time.time;
            }
#endif

            if (Time.time - lastEnableTime_ > DISABLE_TIME)
            {
                return null;
            }

            return landmarkArray_[id, index];
        }

        public int GetHandednesses(int id)
        {
            int ret;

#if UNITY_ANDROID && !UNITY_EDITOR
            ret = multiHandMain_.Call<int>("getHandednesses", id);
#elif UNITY_IOS && !UNITY_EDITOR
            ret = multiHandGetHandednesses(id);
#else
            ret = -1;
#endif

            return ret;
        }

        public Vector3 GetHandDirection(int id)
        {
            Vector3 ret = rotationQuaternions_[id] * Vector3.forward;
            if (GetHandednesses(id) != 0)
            {
                ret = Quaternion.Euler(0f, 180f, 0f) * ret;
            }
            return ret;
        }

        public Quaternion GetHandRotation(int id)
        {
            Quaternion ret = rotationQuaternions_[id];
            if (GetHandednesses(id) != 0)
            {
                ret = Quaternion.Euler(0f, 180f, 0f) * ret;
            }
            return ret;
        }

        void OnDestroy()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (multiHandMain_ != null)
            {
                multiHandMain_.Dispose();
            }
#endif

#if UNITY_IOS && !UNITY_EDITOR
            multiHandCleanup();
#endif

            hand3dReset();
        }
    }
}
