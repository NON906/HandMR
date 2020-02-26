using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class HandVRMain : MonoBehaviour
{
    const int RESIZE_HEIGHT = 512;

    public RenderTexture InputRenderTexture;
    public float ShiftX = 0f;
    public float ShiftY = 0f;

#if UNITY_ANDROID
    AndroidJavaObject multiHandMain_;
#endif

#if UNITY_IOS
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
#endif

    bool isStart_ = false;
    Texture2D texture2D_;

    void Start()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentUnityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            multiHandMain_ = new AndroidJavaObject("online.mumeigames.mediapipe.apps.multihandtrackinggpu.MultiHandMain", currentUnityActivity);
        }
#endif

        texture2D_ = new Texture2D(RESIZE_HEIGHT * Screen.width / Screen.height, RESIZE_HEIGHT, TextureFormat.ARGB32, false, false);

#if UNITY_IOS
        IntPtr graphName = Marshal.StringToHGlobalAnsi("multihandtrackinggpu");
        multiHandSetup(graphName, texture2D_.width, texture2D_.height);
        Marshal.FreeHGlobal(graphName);
#endif
    }

    void Update()
    {
        updateFrame();
    }

    void updateFrame()
    {

#if UNITY_ANDROID
        if (!isStart_)
        {
            multiHandMain_.Call("startRunningGraph");
            isStart_ = true;
        }
#endif

#if UNITY_IOS
        if (!isStart_)
        {
            multiHandStartRunningGraph();
            isStart_ = true;
        }
#endif

        RenderTexture reshapedTexture = RenderTexture.GetTemporary(texture2D_.width, texture2D_.height);
        Graphics.Blit(InputRenderTexture, reshapedTexture);

        RenderTexture.active = reshapedTexture;
        texture2D_.ReadPixels(new Rect(0, 0, texture2D_.width, texture2D_.height), 0, 0, false);
        texture2D_.Apply();
        RenderTexture.active = null;

        RenderTexture.ReleaseTemporary(reshapedTexture);

        byte[] frameImage;

#if UNITY_ANDROID
        frameImage = ImageConversion.EncodeToJPG(texture2D_);

        sbyte[] frameImageSigned = Array.ConvertAll(frameImage, b => unchecked((sbyte)b));
        multiHandMain_.Call("setFrame", frameImageSigned);
#endif

#if UNITY_IOS
        frameImage = ImageConversion.EncodeToPNG(texture2D_);

        IntPtr frameIntPtr = Marshal.AllocHGlobal(frameImage.Length * Marshal.SizeOf<byte>());
        Marshal.Copy(frameImage, 0, frameIntPtr, frameImage.Length);
        multiHandSetFrame(frameIntPtr, frameImage.Length);
        Marshal.FreeHGlobal(frameIntPtr);
#endif
    }

    public float[] GetLandmark(int id, int index)
    {
        float[] posVecArray = null;

#if UNITY_ANDROID
        posVecArray = multiHandMain_.Call<float[]>("getLandmark", id, index);
        if (posVecArray == null)
        {
            return null;
        }
#endif

#if UNITY_IOS
        if (multiHandGetHandCount() <= id)
        {
            return null;
        }
        posVecArray = new float[3];
        for (int loop = 0; loop < 3; loop++)
        {
            posVecArray[loop] = multiHandGetLandmark(id, index, loop);
        }
#endif

        posVecArray[0] = (posVecArray[0] - 0.5f) * 0.15f * Screen.width / Screen.height + ShiftX;
        posVecArray[1] = (posVecArray[1] - 0.5f) * -0.15f + ShiftY;
        posVecArray[2] = posVecArray[2] * 0.001f + 0.3f;

        return posVecArray;
    }

    void OnDestroy()
    {
#if UNITY_ANDROID
        if (multiHandMain_ != null)
        {
            multiHandMain_.Dispose();
        }
#endif

#if UNITY_IOS
        multiHandCleanup();
#endif
    }
}
