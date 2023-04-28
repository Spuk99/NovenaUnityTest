using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JSONReader : MonoBehaviour
{
    [SerializeField]
    private GameObject languageButton; //prefab of langageButton for page 1
    [SerializeField]
    private GameObject listButton; // prefab of listButton for page 2

    [SerializeField]
    private Button backListButton; // button to go back to page 1 
    [SerializeField]
    private Button backdetailsButton; // button to go back to page 2

    [SerializeField]
    private GameObject contentLanguage; // content of the scrollview for page 1
    [SerializeField]
    private GameObject contentList; // content of the scrollview for page 2

    [SerializeField]
    private GameObject titleDetails; // title of the page 3 
    [SerializeField]
    private GameObject audioDetails; // parent of the slider for audio
    [SerializeField]
    private Image loadImage; // empty image to load the image of the page 3 - gallery 
    [SerializeField]
    private GameObject playStopButton; //button for playing and pausing the audio
    [SerializeField]
    private Image playImage; //image for play icon for playStopButton
    [SerializeField]
    private Image stopImage; //image for stop icon for playStopButton

    [SerializeField]
    private GameObject languagePanel; // panel for page 1 - Language page
    [SerializeField]
    private GameObject listPanel; // panel for page 2 - List page
    [SerializeField]
    private GameObject detailsPanel; // panel for page 3 - Details page
    [SerializeField]
    private GameObject detTextPanel; // panel for page 3 - Details page - text

    [SerializeField]
    private AudioSource audioSource; // Audio sorce for playing audio

    private Slider slider; // slider for fetching the slider component from audioDetails

    //TranslatedContentsData clas - for storing the data from json
    [System.Serializable]
    public class TranslatedContentsData
    {
        public int LanguageId;
        public string LanguageName;
        public Topic[] Topics;
    }
    // Topic class - for storing the data from json - stores Topic data
    [System.Serializable]
    public class Topic
    {
        public string Name;
        public Media[] Media;
        public string Details;
    }
    // Media class - for storing the data from json - stores Media data
    [System.Serializable]
    public class Media
    {
        public string Name;
        public string FilePath;
        public Photo[] Photos;
    }
    //Photo class - for storing the data from json - stores Photo data
    [System.Serializable]
    public class Photo
    {
        public string Path;
        public string Name;
    }

    //JsonDataList for turning the clas into a list
    [System.Serializable]
    public class JsonDataList
    {
        public TranslatedContentsData[] TranslatedContents;
    }

    public JsonDataList myJsonDataList = new JsonDataList();

    private void Awake()
    {
        //Call coroutine GetRequest - fetch the json file from Application.streamingAssetsPath and write into Application.persistentDataPath
        StartCoroutine(GetRequest(Path.Combine(Application.streamingAssetsPath, "example.json")));  
    }

    void Start()
    {
        playStopButton.GetComponent<Button>().onClick.AddListener(PlayStopAudio);
    }

    //Loading of files:
    //Load JSON file into Application.persistentDaatPath. Call coroutine LoadingJson.
    IEnumerator GetRequest(string path)
    {
        WWW loadDB = new WWW(path);
        yield return loadDB.text;
        File.WriteAllBytes(Application.persistentDataPath + "/example.json", loadDB.bytes);
        yield return StartCoroutine(LoadingJson());
    }
    // Load the json file from Application.persistentDataPath and store it into myJsonDataList. Call GenerateLanguageButtons and coroutine GetEverythingElse()
    IEnumerator LoadingJson()
    {
        string path = Application.persistentDataPath + "/example.json";
        if (File.Exists(path))
        {
            byte[] jsonData = File.ReadAllBytes(path);
            string jsonStr = Encoding.ASCII.GetString(jsonData);
            myJsonDataList = JsonUtility.FromJson<JsonDataList>(jsonStr);
        }
        GenerateLanguageButton();
        yield return StartCoroutine(GetEverythingElse());
    }

    //Go through the myJsonDataList and load meadia (Audio and photo) from Application.streamingAssetsPath into Application.persistentDataPath with coroutine GetFile
    IEnumerator GetEverythingElse()
    {
        string destination = Application.persistentDataPath;
        foreach (TranslatedContentsData item in myJsonDataList.TranslatedContents)
        {
            Topic[] topics = item.Topics;
            foreach (Topic it in topics)
            {
                Media[] media = it.Media;
                foreach (Media i in media)
                {
                    if (i.Name == "Audio")
                    {
                        StartCoroutine(GetFile(Path.Combine(Application.streamingAssetsPath, i.FilePath), destination, i.FilePath));
                    }
                    else
                    {
                        Photo[] photos = i.Photos;
                        foreach (Photo p in photos)
                        {
                            StartCoroutine(GetFile(Path.Combine(Application.streamingAssetsPath, p.Path), destination, p.Path));
                        }
                    }
                }
            }
        }
        yield return null;
    }
    //Load the file from Application.streamingAssetsPath and write it into Application.persistentDataPath
    IEnumerator GetFile(string path, string destination, string orgPath)
    {
        WWW loadDB = new WWW(path);
        yield return loadDB.bytes;
        File.WriteAllBytes(destination + "/"+ orgPath, loadDB.bytes);
    }

    //Generate Language buttons for page 1 from myJsonDataList
    //Generated buttons have two listeners, one for panel switching, one for loading the list of the topic buttons for page 2.
    public void GenerateLanguageButton()
    {
        foreach(TranslatedContentsData item in myJsonDataList.TranslatedContents)
        {
            GameObject obj = Instantiate(languageButton, contentLanguage.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = item.LanguageName;
            obj.GetComponent<Button>().onClick.AddListener(() => ChangePanels(languagePanel, listPanel));
            obj.GetComponent<Button>().onClick.AddListener(() => GenerateListButtons(item.LanguageId - 1));
        }
    }

    //Changeing betwwen panels - used for all generated buttons - switch from page 1 to page 2 and from page 2 to page 3
    void ChangePanels(GameObject current, GameObject next)
    {
        current.SetActive(false);
        next.SetActive(true);
    }

    //Generate List buttons for page 2 from myJsonDataList from specific language that was selected.
    //Buttons have two listeners, one for panel switching, one for loading the details of the topic buttons for page 3.
    public void GenerateListButtons(int index)
    {
        int counter = 1;
        Topic[] topics = myJsonDataList.TranslatedContents[index].Topics;
        
        foreach (Topic item in topics)
        {
            GameObject obj = Instantiate(listButton, contentList.transform);
            obj.GetComponentsInChildren<TextMeshProUGUI>()[0].text = item.Name;
            obj.GetComponentsInChildren<TextMeshProUGUI>()[1].text = counter.ToString();
            obj.GetComponent<Button>().onClick.AddListener(() => ChangePanels(listPanel, detailsPanel));
            int num = counter;
            obj.GetComponent<Button>().onClick.AddListener(() => SetUpDetails(item,num.ToString()));
            counter++;
        }
        backListButton.onClick.AddListener(() => DestroyObjects(contentList));
    }

    //Destroy instantiated objects when changing panels - for going from page 2 to page 1
    public void DestroyObjects(GameObject content)
    {
        Transform[] listChild = content.GetComponentsInChildren<Transform>();
        for(int i=1; i<listChild.Length; i++)
        {
            UnityEngine.Object.Destroy(listChild[i].gameObject);
        }
    }

    //Load title, audio and gallery for page 3 from myJsonDataList from specific topic that was selected.
    //Both audio and images for the gallery are loaded from coroutines. The gallery switching is also implemented through a coroutine
    public void SetUpDetails(Topic top, string number)
    {
        TextMeshProUGUI[] texts = titleDetails.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = number;
        texts[1].text = top.Name;
        detTextPanel.GetComponentInChildren<TextMeshProUGUI>().text = top.Details;
        
        Media[] med = top.Media;
        Photo[] phot = { };
        string duration="";

        slider = audioDetails.GetComponentInChildren<Slider>();

        //load audio from Media from Topic
        foreach (Media item in med)
        {
            if (item.Name == "Audio")
            {
                StartCoroutine(LoadAudioUrl(Path.Combine(Application.persistentDataPath, item.FilePath), duration)); 
            }
            //load Gallery from Media from Topic
            else if (item.Name == "Gallery")
            {
                phot = item.Photos;
            }
        }
        
        StartCoroutine(LoadGallery(phot));
        backdetailsButton.onClick.AddListener(() => ResetEverything());
    }

    //Load audio from Application.persistentDataPath  + Update the proress bar of audio and text that shows the lenght of the audio
    IEnumerator LoadAudioUrl(string url, string duration)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + url, AudioType.MPEG);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning("Audio error:" + www.error);
        }
        else
        {
            AudioClip audioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            Debug.Log(audioClip.name);
            audioSource.clip = audioClip;
            TimeSpan ts = TimeSpan.FromSeconds(audioSource.clip.length);
            double minutes = ts.Minutes;
            double seconds = ts.Seconds;
            string secStr;
            string minStr;
            if (minutes < 10)
            {
                minStr = "0" + minutes.ToString();
            }
            else
            {
                minStr = minutes.ToString();
            }
            if (seconds < 10)
            {
                secStr = "0" + seconds.ToString();
            }
            else
            {
                secStr = seconds.ToString();
            }
            duration = minStr + ":" + secStr;
            audioDetails.GetComponentInChildren<TextMeshProUGUI>().text = "00:00 / " + duration;
            slider.maxValue = audioSource.clip.length;
            audioSource.Play();
        }
        yield return StartCoroutine(UpdateAudioTime(audioDetails.GetComponentInChildren<TextMeshProUGUI>(), duration));
    }
    IEnumerator UpdateAudioTime(TextMeshProUGUI text, string duration)
    {
        while (true)
        {
            TimeSpan ts = TimeSpan.FromSeconds(audioSource.time);
            double min = ts.Minutes;
            double sec = ts.Seconds;
            string secStr;
            string minStr;
            if (min < 10)
            {
                minStr = "0" + min.ToString();
            }
            else
            {
                minStr = min.ToString();
            }
            if (sec < 10)
            {
                secStr = "0" + sec.ToString();
            }
            else
            {
                secStr = sec.ToString();
            }
            string curDur = minStr + ":" + secStr;
            text.text = curDur + " / " + duration;
            if (slider.value < audioSource.clip.length)
            {
                slider.value = (float)audioSource.time;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    //Load images from Application.persistentDataPath for the Gallery. Call a function to call a coroutine that loads the image and then wait 5 seconds in the first coroutine
    IEnumerator LoadGallery(Photo[] phot)
    {
        string path = "file:///" + Application.persistentDataPath + "/";
        foreach (Photo item in phot)
        {
            LoadOfImages(path+item.Path);
            yield return new WaitForSeconds(6);
        }
    }
    public void LoadOfImages(string uri)
    {
        StartCoroutine(LoadSpriteUrl(uri));
    }
    IEnumerator LoadSpriteUrl(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning("Texture error:" + www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            loadImage.sprite = sprite;
        }
    }

    //Play or pause audio on click of PlayStopButton. Changeing of the sprite image
    public void PlayStopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            playImage.gameObject.SetActive(true);
            stopImage.gameObject.SetActive(false);
        }
        else
        {
            audioSource.Play();
            playImage.gameObject.SetActive(false);
            stopImage.gameObject.SetActive(true);
        }
    }

    //On back button click reset everything and stop coroutines
    public void ResetEverything()
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.time = 0;
        playImage.gameObject.SetActive(false);
        stopImage.gameObject.SetActive(true);
        detTextPanel.SetActive(false);
        slider.value = 0;
        audioDetails.GetComponentInChildren<TextMeshProUGUI>().text = "00:00 / 00:00";
        StopAllCoroutines();
    }
}