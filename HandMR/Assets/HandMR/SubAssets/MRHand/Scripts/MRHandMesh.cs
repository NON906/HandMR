using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandMR
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MRHandMesh : MonoBehaviour
    {
        HandVRSphereHand sphereHand_;
        MeshRenderer meshRenderer_;

        void Start()
        {
            sphereHand_ = GetComponentInParent<HandVRSphereHand>();
            meshRenderer_ = GetComponent<MeshRenderer>();
            meshRenderer_.enabled = false;
        }

        void LateUpdate()
        {
            if (sphereHand_.IsTrackingHand)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = new Vector3[] {
                sphereHand_.GetFinger(0).localPosition,
                sphereHand_.GetFinger(1).localPosition,
                sphereHand_.GetFinger(5).localPosition,
                sphereHand_.GetFinger(9).localPosition,
                sphereHand_.GetFinger(13).localPosition,
                sphereHand_.GetFinger(17).localPosition
            };
                mesh.triangles = new int[] {
                0, 5, 4,
                0, 4, 3,
                0, 3, 2,
                0, 2, 1,
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5
            };
                mesh.RecalculateNormals();
                MeshFilter filter = GetComponent<MeshFilter>();
                filter.sharedMesh = mesh;

                meshRenderer_.enabled = true;
            }
            else
            {
                meshRenderer_.enabled = false;
            }
        }
    }
}
