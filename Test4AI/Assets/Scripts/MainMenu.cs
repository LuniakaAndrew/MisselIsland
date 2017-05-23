using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public static bool isMenu;
    public static float soundValue;
    public static float musicValue;

    public Scrollbar music;
    public Scrollbar sound;
    public Toggle screenMade;
    public Text resolutionsText;
    public Text qualityText;

    public GameObject[] allUI;
    public GameObject startMenu;

    public float fadeSpeed = 1.5f;
    public Image _image;

    private Resolution[] resolutionsList;
    private string[] qualityList;
    private bool isFullScreen;
    private int resolutions_id;
    private int quality_id;
    bool sceneStarting;
    // Use this for initialization
    void Start () {

        qualityList = QualitySettings.names;
        resolutionsList = Screen.resolutions;
        quality_id = QualitySettings.GetQualityLevel();
        resolutions_id = 0;
        for (int i = 0; i < resolutionsList.Length; i++)
        {
            if (resolutionsList[i].height == Screen.height && resolutionsList[i].width == Screen.width)
                resolutions_id = i;
            
        }
        
        if (!Screen.fullScreen)
            isFullScreen = false;
        else
            isFullScreen = true;
        foreach (GameObject obj in allUI)
        {
            obj.SetActive(false);
        }

        DefaultSettings();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !isMenu)
            Lock();
        else 
        if (Input.GetKeyDown(KeyCode.Escape) && isMenu)
            UnLock();

        if (sceneStarting)EndScene();

    }
    void DefaultSettings()
    {
        soundValue = 1;
        musicValue = 0.5f;
        LoadSaveSettings(false);
    }
    void LoadSaveSettings(bool isSave)
    {
       /* bool load = true;
        if (!isSave)
        {
            if (PlayerPrefs.HasKey("soundValue"))
                soundValue = PlayerPrefs.GetFloat("soundValue");
            else
                load = false;
            if (PlayerPrefs.HasKey("musicValue"))
                musicValue = PlayerPrefs.GetFloat("musicValue");
            else
                load = false;
        }
        if (!load || isSave)
        {
            PlayerPrefs.SetFloat("soundValue", soundValue);
            PlayerPrefs.SetFloat("musicValue", musicValue);
        }
        */
        //Ready();
    }
    void Ready()
    {
        qualityText.text = qualityList[quality_id];
        resolutionsText.text = resolutionsList[resolutions_id].width + "x" + resolutionsList[resolutions_id].height;
        screenMade.isOn = isFullScreen;
        music.value = musicValue;
        sound.value = soundValue;
    }
    void OnApplicationQuit()
    {
        LoadSaveSettings(true);
        Debug.Log("Application Ready to Close");
    }
    void Lock()
    {
        Time.timeScale = 0;
        startMenu.SetActive(true);
        isMenu = true;
    }

    public void UnLock()
    {
        Time.timeScale = 1;
        isMenu = false;
        LoadSaveSettings(true);
        foreach (GameObject obj in allUI)
        {
            obj.SetActive(false);
        }
    }
    public void AppQuit()
    {
        Application.Quit();
        Debug.Log("Application Close");
    }

    public void SetScreenMode(bool mode)
    {
        isFullScreen = mode;
    }

    public void SoundScrollBar(float value)
    {
        soundValue = value;
    }

    public void MusicScrollBar(float value)
    {
        musicValue = value;
    }

    public void NextResolutionsID()
    {
        if (resolutions_id < resolutionsList.Length - 1) resolutions_id++; else resolutions_id = 0;
        resolutionsText.text = resolutionsList[resolutions_id].width + "x" + resolutionsList[resolutions_id].height;
    }

    public void NextQualityID()
    {
        if (quality_id < qualityList.Length - 1) quality_id++; else quality_id = 0;
        qualityText.text = qualityList[quality_id];
    }

    public void ApplySetting()
    {
        QualitySettings.SetQualityLevel(quality_id, true);
        Screen.SetResolution(resolutionsList[resolutions_id].width, resolutionsList[resolutions_id].height, isFullScreen);
        Screen.fullScreen = isFullScreen;
        LoadSaveSettings(true);
        EnterMainMenu();
    }

    public void EnterSettingsMenu() {
        allUI[0].SetActive(true);
        startMenu.SetActive(false);
    }
    public void EnterMainMenu()
    {
        allUI[0].SetActive(false);
        startMenu.SetActive(true);
    }
    public void StartGame()
    {
        //SceneManager.LoadScene("Test3"); 
        sceneStarting = true;
    }
    void EndScene()
    {
        _image.enabled = true;
        _image.color = Color.Lerp(_image.color, Color.black, fadeSpeed* Time.deltaTime);

        if (_image.color.a >= 0.95f)
        {
            Cursor.visible = true;
            sceneStarting = false;
            _image.color = Color.black;
            Debug.Log("Run");
            SceneManager.LoadScene("Test3");
        }
    }
    

}
