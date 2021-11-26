using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HandMR
{
    [RequireComponent(typeof(Text))]
    public class BackButtonForIOS : MonoBehaviour
    {
        Text text_;

        void Start()
        {
            text_ = GetComponent<Text>();

#if UNITY_ANDROID && !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }

        void Update()
        {
#if DOWNLOADED_ARFOUNDATION
            float touchX;
            float touchY;

            var pointer = UnityEngine.InputSystem.Pointer.current;
            if (pointer != null && pointer.press != null && pointer.press.wasPressedThisFrame)
            {
                touchX = pointer.position.ReadValue().x;
                touchY = pointer.position.ReadValue().y;
            }
            else
            {
                return;
            }

            if ((text_.rectTransform.position.x - text_.rectTransform.rect.width * 0.5f) < touchX && touchX < (text_.rectTransform.position.x + text_.rectTransform.rect.width * 0.5f) &&
                (text_.rectTransform.position.y - text_.rectTransform.rect.height * 0.5f) < touchY && touchY < (text_.rectTransform.position.y + text_.rectTransform.rect.height * 0.5f))
            {
                SceneManager.LoadScene("Menu");
            }
#endif
        }
    }
}
