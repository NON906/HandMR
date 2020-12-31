using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace HandMR
{
    public static class ChangeLanguage
    {
        public enum Languages
        {
            English,
            Japanese
        }

        [Serializable]
        class LanguageJsonArray
        {
            public LanguageJson[] lang = null;
        }

        [Serializable]
        class LanguageJson
        {
            public string en = "";
            public string jp = "";
        }

        class LanguageSentence
        {
            public string Before
            {
                get;
                set;
            }
            public string After
            {
                get;
                set;
            }
        }

        static string[] SCENES = new string[]
        {
        "Assets/HandMR/Sample/Scenes/Menu.unity",
        "Assets/HandMR/Sample/Scenes/License.unity",
        "Assets/HandMR/Sample/Scenes/HandSizeCalib.unity",
        "Assets/HandMR/Sample/Scenes/Main.unity",
        "Assets/HandMR/Sample/HandFireBall/Scenes/Main.unity",
        };

        static string removeSymbolFromString(string symbols, string remove)
        {
            symbols = symbols.Replace(";" + remove, "");
            if (symbols == remove)
            {
                symbols = "";
            }
            if (symbols.StartsWith(remove + ";"))
            {
                symbols = symbols.Substring(remove.Length + 1);
            }
            return symbols;
        }

        static void removeScriptingDefineSymbol(string remove)
        {
            string symbols;
            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            symbols = removeSymbolFromString(symbols, remove);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);

            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
            symbols = removeSymbolFromString(symbols, remove);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbols);

            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            symbols = removeSymbolFromString(symbols, remove);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
        }

        static public void Change(Languages before, Languages after)
        {
            string settingsStr = File.ReadAllText("Assets/HandMR/Settings/language.json");
            LanguageJson[] settings = JsonUtility.FromJson<LanguageJsonArray>(settingsStr).lang;

            List<LanguageSentence> languageSentences = new List<LanguageSentence>();
            foreach (var setting in settings)
            {
                var newSentence = new LanguageSentence();

                switch (before)
                {
                    case Languages.English:
                        newSentence.Before = setting.en;
                        break;
                    case Languages.Japanese:
                        newSentence.Before = setting.jp;
                        break;
                }

                switch (after)
                {
                    case Languages.English:
                        newSentence.After = setting.en;
                        break;
                    case Languages.Japanese:
                        newSentence.After = setting.jp;
                        break;
                }

                languageSentences.Add(newSentence);
            }

            foreach (string scene in SCENES)
            {
                if (!File.Exists(scene))
                {
                    continue;
                }

                Scene sceneObj = EditorSceneManager.OpenScene(scene);

                var texts = UnityEngine.Object.FindObjectsOfType<Text>();
                foreach (var text in texts)
                {
                    var sentence = languageSentences.SingleOrDefault(x => x.Before == text.text);
                    if (sentence != null)
                    {
                        text.text = sentence.After;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(text);
                    }
                }

                var dropdowns = UnityEngine.Object.FindObjectsOfType<Dropdown>();
                foreach (var dropdown in dropdowns)
                {
                    bool isChange = false;
                    foreach (var option in dropdown.options)
                    {
                        var sentence = languageSentences.SingleOrDefault(x => x.Before == option.text);
                        if (sentence != null)
                        {
                            option.text = sentence.After;
                            isChange = true;
                        }
                    }
                    if (isChange)
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(dropdown);
                    }
                }

                EditorSceneManager.SaveScene(sceneObj);
            }

            switch (before)
            {
                case Languages.English:
                    removeScriptingDefineSymbol("LANG_EN");
                    break;
                case Languages.Japanese:
                    removeScriptingDefineSymbol("LANG_JP");
                    break;
            }

            switch (after)
            {
                case Languages.English:
                    PackagesPostprocessor.SetScriptingDefineSymbol("LANG_EN");
                    break;
                case Languages.Japanese:
                    PackagesPostprocessor.SetScriptingDefineSymbol("LANG_JP");
                    break;
            }

        }
    }
}
