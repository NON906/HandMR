using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HandMR
{
    public class BackButtonExit : MonoBehaviour
    {
        void Update()
        {
#if DOWNLOADED_ARFOUNDATION
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Exit();
                return;
            }
#endif
        }

        public void Exit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
