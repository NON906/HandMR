using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HandMR
{
    public class HandVRPosition : MonoBehaviour
    {
        const float DISABLE_TIME = 0.1f;

        public int Index;

        public HandVRSphereHand.EitherHand ThisEitherHand
        {
            set;
            get;
        }

        public bool PhysicEnabled
        {
            get;
            private set;
        } = false;

        HandVRMain handVRMain_;
        Vector3 position_;
        Rigidbody rigidbody_;
        Renderer renderer_;
        float currentDisableTime_ = 0f;

        void Start()
        {
            handVRMain_ = FindObjectOfType<HandVRMain>();
            rigidbody_ = GetComponent<Rigidbody>();
            renderer_ = GetComponent<Renderer>();
            if (renderer_ != null)
            {
                renderer_.enabled = false;
            }
        }

        void Update()
        {
            if (handVRMain_ == null)
            {
                handVRMain_ = FindObjectOfType<HandVRMain>();
            }
            if (handVRMain_ == null)
            {
                return;
            }

            int id = handVRMain_.GetIdFromHandednesses(ThisEitherHand);
            if (id < 0)
            {
                PhysicEnabled = false;
                if (Time.unscaledTime > currentDisableTime_)
                {
                    if (renderer_ != null)
                    {
                        renderer_.enabled = false;
                    }
                }
                return;
            }

            float[] posVecArray = handVRMain_.GetLandmark(id, Index);
            if (posVecArray != null)
            {
                if (renderer_ != null)
                {
                    renderer_.enabled = true;
                }
                PhysicEnabled = true;

                position_ = new Vector3(posVecArray[0], posVecArray[1], posVecArray[2]);
                currentDisableTime_ = Time.unscaledTime + DISABLE_TIME;
            }
            else
            {
                PhysicEnabled = false;
                if (Time.unscaledTime > currentDisableTime_)
                {
                    if (renderer_ != null)
                    {
                        renderer_.enabled = false;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (rigidbody_ != null && !rigidbody_.isKinematic && PhysicEnabled)
            {
                rigidbody_.AddForce((position_ - transform.localPosition) / Time.fixedDeltaTime - rigidbody_.velocity, ForceMode.VelocityChange);
            }
            else
            {
                transform.localPosition = position_;
            }
        }
    }
}
