using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{

    public InputField inputField;
    string nickname ="";
    private void Awake()
    {
        string obj = SaveSystem.LoadNickname();
        // Debug.Log("value login >>> " + obj);
        if (obj != null && !obj.Equals(""))
        {
            Debug.Log(obj);
            loadScene(1);
        }

    }
    public void loadScene(int levelName)
    {
        SceneManager.LoadScene(levelName);
    }


    public void SetNickname()
    {
        if (inputField.text.Length > 2)
        {
            nickname = inputField.text;
            SaveSystem.SaveNickname(nickname);
            Debug.Log(nickname);
            loadScene(1);
        }
    }
}
