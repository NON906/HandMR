using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    public class Fisheye : MonoBehaviour
    {
        public Material FisheyeMaterial;
        public bool IsLeft = false;

        public float Rate
        {
            get
            {
                return FisheyeMaterial.GetFloat("_Rate");
            }
            set
            {
                FisheyeMaterial.SetFloat("_Rate", value);
            }
        }

        public float Center
        {
            get
            {
                float center = FisheyeMaterial.GetFloat("_Center");
                if (IsLeft)
                {
                    center = 1f - center;
                }
                return center;
            }
            set
            {
                float center = value;
                if (IsLeft)
                {
                    center = 1f - center;
                }
                FisheyeMaterial.SetFloat("_Center", center);
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (!enabled)
            {
                return;
            }

            Graphics.Blit(src, dest, FisheyeMaterial);
        }
    }
}
