using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeBackGroundQuad : MonoBehaviour
{
    public Canvas NoticeTextCanvas;

    void Start()
    {
        float fov = 60f * Mathf.Deg2Rad;
        float distance = transform.localPosition.z;
        float height = 2f * distance * Mathf.Tan(fov / 2f);

        float fovWidth = 2f * Mathf.Atan(height * Screen.width / Screen.height / 2f / distance);
        float width = 2f * distance * Mathf.Tan(fovWidth / 2f);

        transform.localScale = new Vector3(width, height, 1f);

        NoticeTextCanvas.transform.localScale = new Vector3(NoticeTextCanvas.transform.localScale.x * width, NoticeTextCanvas.transform.localScale.y * width, 1f);
        NoticeTextCanvas.transform.localPosition = new Vector3(transform.localPosition.x, height * -0.5f, transform.localPosition.z);
    }
}