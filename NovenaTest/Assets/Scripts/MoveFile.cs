using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoveFile : MonoBehaviour
{
    //method on awake
    private void Awake()
    {
        //creat txt file in Application.persistentDataPath and write done in it
        File.WriteAllText(Application.persistentDataPath + "/done.txt", "done\n");
        //creat folder named files in Application.persistentDataPath
        Directory.CreateDirectory(Application.persistentDataPath + "/files");
        File.WriteAllText(Application.persistentDataPath + "/done.txt", "created folder files\n");

        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Application.persistentDataPath);

        //Copy file from Application.streamingAssetsPath to Application.persistentDataPath
        File.Copy(Path.Combine(Application.streamingAssetsPath, "example.json"), Path.Combine(Application.persistentDataPath, "example.json"), true);

        //copy contents of folder files from Application.streamingAssetsPath to Application.persistentDataPath to folder named files
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/files");
        foreach (FileInfo file in dir.GetFiles())
        {
            string temppath = Path.Combine(Application.persistentDataPath + "/files", file.Name);
            file.CopyTo(temppath, true);
        }
    }
}
