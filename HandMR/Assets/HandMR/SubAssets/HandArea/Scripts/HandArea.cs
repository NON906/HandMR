using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class HandArea : MonoBehaviour
    {
        public float Size = 0.8f;
        public float ColorAlphaChangeSpeed = 0.1f;

        HandVRMain handVRMain_;
        HandMRManager handMRManager_;
        Renderer renderer_;

        Color defaultColor_;
        Color defaultColorInner_;
        float colorAlpha_ = 0f;
        bool renderEnabled_ = false;
        bool isSetThickness_ = false;

        void Start()
        {
            handVRMain_ = FindObjectOfType<HandVRMain>();
            handMRManager_ = FindObjectOfType<HandMRManager>();
            renderer_ = transform.GetComponentInChildren<Renderer>();

            defaultColor_ = renderer_.material.GetColor("_Color");
            renderer_.material.SetColor("_Color",
                new Color(defaultColor_.r, defaultColor_.g, defaultColor_.b, 0f));
            defaultColorInner_ = renderer_.material.GetColor("_ColorInner");
            renderer_.material.SetColor("_ColorInner",
                new Color(defaultColorInner_.r, defaultColorInner_.g, defaultColorInner_.b, 0f));
        }

        void Update()
        {
            if (handMRManager_ == null)
            {
                return;
            }

            renderEnabled_ = false;
            foreach (var hand in handMRManager_.Hands)
            {
                renderEnabled_ = hand.GetComponent<HandVRSphereHand>().IsTrackingHand;
                if (renderEnabled_)
                {
                    break;
                }
            }
            renderEnabled_ = renderEnabled_ && handMRManager_.CurrentViewMode == HandMRManager.Mode.VR;
        }

        void FixedUpdate()
        {
            if (renderEnabled_)
            {
                colorAlpha_ = colorAlpha_ * (1f - ColorAlphaChangeSpeed) + ColorAlphaChangeSpeed;
            }
            else
            {
                colorAlpha_ = 0f;
            }
        }

        void LateUpdate()
        {
            if (handMRManager_ == null)
            {
                return;
            }

            if (colorAlpha_ > 0f)
            {
                Transform cameraTrans = handMRManager_.GetCameraTransform();

                float zLength = 0f;
                int zLengthCount = 0;
                foreach (var hand in handMRManager_.Hands)
                {
                    var sHand = hand.GetComponent<HandVRSphereHand>();
                    if (sHand.IsTrackingHand)
                    {
                        zLength += cameraTrans.InverseTransformPoint(sHand.HandCenterPosition).z;
                        zLengthCount++;
                    }
                }
                if (zLengthCount <= 0)
                {
                    colorAlpha_ = 0f;
                }
                else
                {
                    if (!isSetThickness_)
                    {
                        renderer_.material.SetFloat("_ThicknessX",
                            renderer_.material.GetFloat("_ThicknessX") * handVRMain_.Height / handVRMain_.Width);
                        renderer_.material.SetFloat("_ThicknessInnerX",
                            renderer_.material.GetFloat("_ThicknessInnerX") * handVRMain_.Height / handVRMain_.Width);

                        isSetThickness_ = true;
                    }

                    zLength /= zLengthCount;

                    float height = Mathf.Tan(handVRMain_.FixedFieldOfView * Mathf.Deg2Rad * 0.5f) * zLength * Size;
                    float width = height * handVRMain_.Width / handVRMain_.Height;
                    transform.localScale = new Vector3(width, height, 1f);

                    Vector3 localPos = new Vector3(handVRMain_.ShiftX, handVRMain_.ShiftY, zLength);
                    transform.position = cameraTrans.TransformPoint(localPos);
                    transform.rotation = cameraTrans.rotation;
                }
            }

            renderer_.material.SetColor("_Color",
                new Color(defaultColor_.r, defaultColor_.g, defaultColor_.b, defaultColor_.a * colorAlpha_));
            renderer_.material.SetColor("_ColorInner",
                new Color(defaultColorInner_.r, defaultColorInner_.g, defaultColorInner_.b, defaultColorInner_.a * colorAlpha_));
        }
    }
}
