using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class JSONReader : MonoBehaviour
{
    [SerializeField]
    private TextAsset textJSON;

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
    private GameObject languagePanel;
    [SerializeField]
    private GameObject listPanel;
    [SerializeField]
    private GameObject detailsPanel;

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

    // Start is called before the first frame update
    void Start()
    {
        myJsonDataList = JsonUtility.FromJson<JsonDataList>(textJSON.text);
        //call method for generating language buttons
        GenerateLanguageButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    //CFhangeing betwwen panels - used for all generated buttons
    void ChangePanels(GameObject current, GameObject next)
    {
        current.SetActive(false);
        next.SetActive(true);
    }

    public void GenerateListButtons(int index)
    {
        Debug.Log("Index je: " + index);
        int counter = 1;
        Topic[] topics = myJsonDataList.TranslatedContents[index].Topics;
        foreach (Topic item in topics)
        {
            GameObject obj = Instantiate(listButton, contentList.transform);
            //obj.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
            obj.GetComponentsInChildren<TextMeshProUGUI>()[0].text = item.Name;
            obj.GetComponentsInChildren<TextMeshProUGUI>()[1].text = counter.ToString();
            obj.GetComponent<Button>().onClick.AddListener(() => ChangePanels(listPanel, detailsPanel));
            counter++;
        }
        backListButton.onClick.AddListener(() => DestroyObjects(contentList));
    }

    public void DestroyObjects(GameObject content)
    {
        Transform[] listChild = content.GetComponentsInChildren<Transform>();
        for(int i=1; i<listChild.Length; i++)
        {
            Object.Destroy(listChild[i].gameObject);
        }
    }
}
