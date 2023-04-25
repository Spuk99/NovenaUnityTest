using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Copy file from Application.streamingAssetsPath to Application.persistentDataPath
        System.IO.File.Copy(Application.streamingAssetsPath + "/example.json", Application.persistentDataPath + "/example.json", true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
