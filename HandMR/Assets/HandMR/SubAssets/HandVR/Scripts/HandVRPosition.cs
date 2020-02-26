using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandVRPosition : MonoBehaviour
{
    public int Index;

    public int Id
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
        float[] posVecArray = handVRMain_.GetLandmark(Id, Index);
        if (posVecArray != null)
        {
            if (renderer_ != null)
            {
                renderer_.enabled = true;
            }
            PhysicEnabled = true;

            position_ = new Vector3(posVecArray[0], posVecArray[1], posVecArray[2]);
        }
        else
        {
            if (renderer_ != null)
            {
                renderer_.enabled = false;
            }
            PhysicEnabled = false;
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
