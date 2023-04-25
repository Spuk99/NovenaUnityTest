using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(Download());
        //start coroutine that accesses the data from application.persistentDataPath
        StartCoroutine(FindFile());
    }

    IEnumerator Download()
    {
        Debug.Log("Download");
        //use UnityWebRequest for downloading
        UnityWebRequest www = UnityWebRequest.Get("https://github.com/Spuk99/tester.git");
        yield return www.SendWebRequest();
        //copy nativeData from www.downloadHandler to Application.persistentDataPath
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/example.json", www.downloadHandler.data);
    }

    IEnumerator FindFile()
    {
        Debug.Log("FindFile");
        //wait until the file is written
        yield return new WaitUntil(() => System.IO.File.Exists(Application.persistentDataPath + "/example.json"));
        //read the file
        string text = System.IO.File.ReadAllText(Application.persistentDataPath + "/example.json");
        Debug.Log(text);
    }
}
