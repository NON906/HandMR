using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HandMR
{
    public class License : MonoBehaviour
    {
        public TextAsset[] LicenseFiles;
        public int[] SkipLines;
        public RectTransform Content;
        public Text TextLinePrefab;

        void Start()
        {
            int fileCnt = 0;
            foreach (var license in LicenseFiles)
            {
                var textLines = license.text.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
                int lineCnt = 0;
                foreach (var textLine in textLines)
                {
                    if (SkipLines.Length > fileCnt && lineCnt < SkipLines[fileCnt])
                    {
                        lineCnt++;
                        continue;
                    }

                    Text uiText = Instantiate(TextLinePrefab);
                    uiText.text = textLine;
                    uiText.transform.parent = Content;
                }
                fileCnt++;
            }
        }

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
