#if ENABLE_URP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HandMR
{
    [RequireComponent(typeof(Volume))]
    public class FisheyeURP : MonoBehaviour
    {
        public bool IsLeft = false;

        VolumeComponent component_;

        public float Intensity
        {
            get
            {
                return component_.parameters[0].GetValue<float>();
            }
            set
            {
                var param = new VolumeParameter<float>();
                param.value = value;
                component_.parameters[0].SetValue(param);
            }
        }

        public float CenterPosition
        {
            get
            {
                return component_.parameters[3].GetValue<Vector2>().x;
            }
            set
            {
                Vector2 vector;
                if (IsLeft)
                {
                    vector = new Vector2(1f - value, 0.5f);
                }
                else
                {
                    vector = new Vector2(value, 0.5f);
                }

                var param = new VolumeParameter<Vector2>();
                param.value = vector;
                component_.parameters[3].SetValue(param);
            }
        }

        public float Scale
        {
            get
            {
                return component_.parameters[4].GetValue<float>();
            }
            set
            {
                var param = new VolumeParameter<float>();
                param.value = value;
                component_.parameters[4].SetValue(param);
            }
        }

        void Awake()
        {
            var volume = GetComponent<Volume>();
            component_ = volume.profile.components[0];
        }
    }
}

#endif