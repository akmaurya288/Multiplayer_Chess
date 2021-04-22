using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

public static class SaveSystem
{

    public static void SaveNickname(string root)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/nickname.krishna";
        FileStream stream = new FileStream(path, FileMode.Create);

        string data = root;
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static string LoadNickname()
    {
        string path = Application.persistentDataPath + "/nickname.krishna";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteAllFiles()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
            f.Delete();
        }
        foreach (FileInfo f in info)
        {
            Debug.Log("file name in system >>> " + f.Name);

        }
        // if (Directory.Exists(path)) { Directory.Delete(path, true); }
        // Directory.CreateDirectory(path);
    }
}