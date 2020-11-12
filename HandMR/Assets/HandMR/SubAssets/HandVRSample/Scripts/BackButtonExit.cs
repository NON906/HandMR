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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
                return;
            }
        }

        public void Exit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
