using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeBackGroundQuad : MonoBehaviour
{
    public Canvas NoticeTextCanvas;
    public bool NoticeTextCenter = false;
    public float FieldOfView = 60f;

    void Start()
    {
        Resize();
    }

    public void Resize()
    {
        float fov = FieldOfView * Mathf.Deg2Rad;
        float distance = transform.localPosition.z;
        float height = 2f * distance * Mathf.Tan(fov / 2f);

        float fovWidth = 2f * Mathf.Atan(height * Screen.width / Screen.height / 2f / distance);
        float width = 2f * distance * Mathf.Tan(fovWidth / 2f);

        transform.localScale = new Vector3(width, height, 1f);

        NoticeTextCanvas.transform.localScale = new Vector3(NoticeTextCanvas.transform.localScale.x * width, NoticeTextCanvas.transform.localScale.y * width, 1f);
        if (NoticeTextCenter)
        {
            NoticeTextCanvas.transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z - 0.01f);
        }
        else
        {
            NoticeTextCanvas.transform.localPosition = new Vector3(transform.localPosition.x, height * -0.5f, transform.localPosition.z);
        }
    }
}