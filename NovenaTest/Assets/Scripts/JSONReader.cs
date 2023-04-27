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
    private TextAsset textJSON; //load this JSON file from START()

    [SerializeField]
    private GameObject languageButton;
    [SerializeField]
    private GameObject listButton;

    [SerializeField]
    private Button backListButton;
    [SerializeField]
    private Button backdetailsButton;

    [SerializeField]
    private GameObject contentLanguage;
    [SerializeField]
    private GameObject contentList;

    [SerializeField]
    private GameObject titleDetails;
    [SerializeField]
    private GameObject audioDetails;
    [SerializeField]
    private Image loadImage;
    [SerializeField]
    private GameObject playStopButton;
    [SerializeField]
    private Image playImage;
    [SerializeField]
    private Image stopImage;

    [SerializeField]
    private GameObject languagePanel;
    [SerializeField]
    private GameObject listPanel;
    [SerializeField]
    private GameObject detailsPanel;
    [SerializeField]
    private GameObject detTextPanel;

    [SerializeField]
    private AudioSource audioSource;

    private Slider slider;

    [System.Serializable]
    public class TranslatedContentsData
    {
        public int LanguageId;
        public string LanguageName;
        public Topic[] Topics;
    }
    [System.Serializable]
    public class Topic
    {
        public string Name;
        public Media[] Media;
        public string Details;
    }
    [System.Serializable]
    public class Media
    {
        public string Name;
        public string FilePath;
        public Photo[] Photos;
    }
    [System.Serializable]
    public class Photo
    {
        public string Path;
        public string Name;
    }

    [System.Serializable]
    public class JsonDataList
    {
        public TranslatedContentsData[] TranslatedContents;
    }

    public JsonDataList myJsonDataList = new JsonDataList();


    private void Awake()
    {
        StartCoroutine(GetRequest(Path.Combine(Application.streamingAssetsPath, "example.json")));
        
        byte[] jsonDataAsBytes = null;
        if (File.Exists(Application.persistentDataPath + "/example.json"))
            jsonDataAsBytes = File.ReadAllBytes(Application.persistentDataPath + "/example.json");
        
        string jsonData = Encoding.ASCII.GetString(jsonDataAsBytes);

        myJsonDataList = JsonUtility.FromJson<JsonDataList>(jsonData);

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
    }

    // Start is called before the first frame update
    void Start()
    {
        //call method for generating language buttons
        GenerateLanguageButton();
        playStopButton.GetComponent<Button>().onClick.AddListener(PlayStopAudio);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GetRequest(string path)
    {
        WWW loadDB = new WWW(path);
        yield return loadDB.text;
        File.WriteAllBytes(Application.persistentDataPath + "/example.json", loadDB.bytes);
    }

    IEnumerator GetFile(string path, string destination, string orgPath)
    {
        WWW loadDB = new WWW(path);
        yield return loadDB.bytes;
        File.WriteAllBytes(destination + "/"+ orgPath, loadDB.bytes);
    }


    //Generate Language buttons for page 1
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

    //Changeing betwwen panels - used for all generated buttons
    void ChangePanels(GameObject current, GameObject next)
    {
        current.SetActive(false);
        next.SetActive(true);
    }
    
    //Generate List buttons for page 2
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
                //audioSource.clip = Resources.Load<AudioClip>(item.FilePath.TrimEnd(".mp3".ToCharArray()));

                StartCoroutine(LoadAudioUrl(Path.Combine(Application.streamingAssetsPath, item.FilePath)));
                
                TimeSpan ts = TimeSpan.FromSeconds(audioSource.clip.length);
                double minutes = ts.Minutes;
                double seconds = ts.Seconds;
                duration = minutes.ToString() + ":" + seconds.ToString();
                audioDetails.GetComponentInChildren<TextMeshProUGUI>().text = "00:00 / " +duration;
                slider.maxValue = audioSource.clip.length;
                audioSource.Play();
                StartCoroutine(UpdateAudioTime(audioDetails.GetComponentInChildren<TextMeshProUGUI>(), duration));
                
            }
            //load Gallery from Media from Topic
            else if (item.Name == "Gallery")
            {
                phot = item.Photos;
            }
        }

        StartCoroutine(LoadImages(phot));
        backdetailsButton.onClick.AddListener(() => ResetEverything());
    }

    IEnumerator LoadAudioUrl(string url)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        yield return www.Send();
        if (www.isError)
        {
            Debug.LogWarning("Audio error:" + www.error);
        }
        else
        {
            AudioClip audioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
            audioSource.clip = audioClip;
        }
    }

    //Update the proress bar of audio and text that shows the lenght of the audio
    IEnumerator UpdateAudioTime(TextMeshProUGUI text, string duration)
    {
        while (true)
        {
            TimeSpan ts = TimeSpan.FromSeconds(audioSource.time);
            double min = ts.Minutes;
            double sec = ts.Seconds;
            string curDur = min.ToString() + ":" + sec.ToString();
            text.text = curDur + " / " + duration;
            if (slider.value < audioSource.clip.length)
            {
                slider.value = (float)audioSource.time;
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    //Load images from Gallery every 5s. After loading the last one it stops.
    IEnumerator LoadImages(Photo[] phot)
    {
        foreach (Photo item in phot)
        {
            loadImage.sprite = Resources.Load<Sprite>(item.Path.TrimEnd(".png".ToCharArray()));
            Debug.Log("Ovo je slika: " + item.Path.TrimEnd(".png".ToCharArray()));
            yield return new WaitForSeconds(5);
        }
    }

    //Play Stop audio on click 
    public void PlayStopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            playImage.gameObject.SetActive(true);
            stopImage.gameObject.SetActive(false);
            Debug.Log("Pause");
        }
        else
        {
            audioSource.Play();
            playImage.gameObject.SetActive(false);
            stopImage.gameObject.SetActive(true);
            Debug.Log("Play");
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
        StopAllCoroutines();
    }
}
