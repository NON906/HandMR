#if DOWNLOADED_ARFOUNDATION

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;

namespace HandMR
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HandMRInputDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'D', 'M', 'R');

        [InputControl(layout = "Button", bit = 0)]
        public bool isTracked;

        [InputControl(name = "devicePosition", layout = "Vector3")]
        public Vector3 position;

        [InputControl(name = "deviceRotation", layout = "Quaternion")]
        public Quaternion rotation;

        [InputControl(layout = "Integer")]
        public int trackingState;

        [InputControl(layout = "Button", bit = 0)]
        public bool isGrab;

        [InputControl(name = "gestureOpen", layout = "Button", bit = 0)]
        [InputControl(name = "gestureClose", layout = "Button", bit = 1)]
        [InputControl(name = "gesturePointer", layout = "Button", bit = 2)]
        [InputControl(name = "gestureGrab", layout = "Button", bit = 3)]
        [InputControl(name = "gestureTwo", layout = "Button", bit = 4)]
        public uint gestures;
    }

    [InputControlLayout(commonUsages = new[] { "LeftHand", "RightHand" }, displayName = "HandMR", stateType = typeof(HandMRInputDeviceState))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class HandMRInputDevice : XRController
    {
        public ButtonControl isGrab { get; private set; }
        public ButtonControl gestureOpen { get; private set; }
        public ButtonControl gestureClose { get; private set; }
        public ButtonControl gesturePointer { get; private set; }
        public ButtonControl gestureGrab { get; private set; }
        public ButtonControl gestureTwo { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            isGrab = GetChildControl<ButtonControl>("isGrab");
            gestureOpen = GetChildControl<ButtonControl>("gestureOpen");
            gestureClose = GetChildControl<ButtonControl>("gestureClose");
            gesturePointer = GetChildControl<ButtonControl>("gesturePointer");
            gestureGrab = GetChildControl<ButtonControl>("gestureGrab");
            gestureTwo = GetChildControl<ButtonControl>("gestureTwo");
        }

        static HandMRInputDevice()
        {
            InputSystem.RegisterLayout<HandMRInputDevice>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInPlayer() { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HandMRHMDDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'H', 'M', 'D');

        [InputControl(layout = "Button", bit = 0)]
        public bool isTracked;

        [InputControl(name = "devicePosition", layout = "Vector3")]
        [InputControl(name = "centerEyePosition", layout = "Vector3")]
        public Vector3 position;

        [InputControl(name = "deviceRotation", layout = "Quaternion")]
        [InputControl(name = "centerEyeRotation", layout = "Quaternion")]
        public Quaternion rotation;

        [InputControl(layout = "Integer")]
        public int trackingState;
    }

    [InputControlLayout(displayName = "HandMR HMD", stateType = typeof(HandMRHMDDeviceState))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class HandMRHMDDevice : XRHMD
    {
        public static HandMRHMDDevice current { get; private set; }

        static HandMRHMDDevice()
        {
            InputSystem.RegisterLayout<HandMRHMDDevice>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInPlayer() { }

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
    }
}

#endif