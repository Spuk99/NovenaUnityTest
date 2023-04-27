using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MoveFile : MonoBehaviour
{
    //method on awake
    private void Awake()
    {
        StartCoroutine(GetRequest(Path.Combine(Application.streamingAssetsPath, "example.json")));     
    }

    IEnumerator GetRequest(string path)
    {        
        WWW loadDB = new WWW(path);
        yield return loadDB.text;
        File.WriteAllBytes(Application.persistentDataPath + "/example.json", loadDB.bytes);
    }
}

