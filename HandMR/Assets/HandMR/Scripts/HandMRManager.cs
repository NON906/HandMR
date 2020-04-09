using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMRManager : MonoBehaviour
{
    public enum Mode
    {
        MR,
        VR,
    };

    [SerializeField]
    Mode ViewMode;

    public GameObject[] VRBackgroundObjects;

    public SetParentMainCamera[] Hands;
    public GameObject MRObject;
    public Transform MRCamera;
    public GameObject VRObject;
    public Transform VRCamera;

    void Start()
    {
        viewModeChange();
    }

    void viewModeChange()
    {
        switch (ViewMode)
        {
            case Mode.MR:
                MRObject.SetActive(true);
                VRObject.SetActive(false);
                foreach (SetParentMainCamera hand in Hands)
                {
                    hand.MainCameraTransform = MRCamera;
                }
                foreach (GameObject obj in VRBackgroundObjects)
                {
                    if (obj != null)
                    {
                        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                        {
                            renderer.enabled = false;
                        }
                    }
                }
                break;
            case Mode.VR:
                MRObject.SetActive(false);
                VRObject.SetActive(true);
                foreach (SetParentMainCamera hand in Hands)
                {
                    hand.MainCameraTransform = VRCamera;
                }
                foreach (GameObject obj in VRBackgroundObjects)
                {
                    if (obj != null)
                    {
                        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
                        {
                            renderer.enabled = true;
                        }
                    }
                }
                break;
        }
    }

    public void ViewModeChange(Mode mode)
    {
        ViewMode = mode;
        viewModeChange();
    }
}
