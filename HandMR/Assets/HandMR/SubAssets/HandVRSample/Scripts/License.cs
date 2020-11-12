using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HandMR
{
    public class License : MonoBehaviour
    {
        public void BackButton()
        {
            SceneManager.LoadScene("Menu");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackButton();
            }
        }
    }
}
