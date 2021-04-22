using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkScript : MonoBehaviour
{
        public static DeepLinkScript Instance { get; private set; }
        public string deeplinkURL;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Application.deepLinkActivated += onDeepLinkActivated;
                if (!String.IsNullOrEmpty(Application.absoluteURL))
                {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    onDeepLinkActivated(Application.absoluteURL);
                }
                // Initialize DeepLink Manager global variable.
                else deeplinkURL = "[none]";
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;

            // Decode the URL to determine action. 
            // In this example, the app expects a link formatted like this:
            // unitydl://mylink?Deep
            string sceneName = url.Split("?"[0])[1];
            bool validScene;
            switch (sceneName)
            {
                case "Local":
                    validScene = true;
                    break;
                case "Deep":
                    validScene = true;
                    break;
                default:
                    validScene = false;
                    break;
            }
            if (validScene) SceneManager.LoadScene(sceneName);
        }

}
