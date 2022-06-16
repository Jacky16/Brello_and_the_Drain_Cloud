using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    bool invertedX = false;
    bool invertedY = true;
    bool isFullscreen = false;

    [Header("Camera")]
    [SerializeField] SettingsCameraPlayer settingsCameraPlayer;
    [SerializeField] Slider sensitivityXSlider;
    [SerializeField] Slider sensitivityYSlider;
    [SerializeField] TMP_InputField sensitivityXInput;
    [SerializeField] TMP_InputField sensitivityYInput;
    [SerializeField] Toggle invertedXToggle;
    [SerializeField] Toggle invertedYToggle;

    [Header("Audio")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;
    [SerializeField] TMP_InputField soundInput;
    [SerializeField] TMP_InputField musicInput;

    [Header("Graphics")]
    [SerializeField] Toggle fullscreenToggle;

    const string soundTag = "SoundVol";
    const string musicTag = "MusicVol";
    const string sensXTag = "SensX";
    const string sensYTag = "SensY";
    const string InvertedX = "InvertedX";
    const string InvertedY = "InvertedY";

    const int defaultVelX = 200;
    const int defaultVelY = 3;

    [Header("MAIN MENU ANIMATIONS")]
    public GameObject MenuAnimManager;
    Animator animManager;
    private void Awake()
    {
        //playerCam = GameObject.FindGameObjectWithTag("CamPlayer").GetComponent<PlayerCam>();
        if (MenuAnimManager)
        {
            animManager = MenuAnimManager.GetComponent<Animator>();
        }
        
    }
    
    private void Start()
    {
        if (MenuAnimManager)
        {           
            animManager.SetBool("isIdle", true);
        }
        EventsUI();
        LoadSettings();
    }
    void EventsUI()
    {
        //Camera
        sensitivityXSlider.onValueChanged.AddListener(ChangeSensitivityXSlider);
        sensitivityYSlider.onValueChanged.AddListener(ChangeSensitivityYSlider);

        sensitivityXInput.onEndEdit.AddListener(ChangeSensitivityXInput);
        sensitivityYInput.onEndEdit.AddListener(ChangeSensitivityYInput);

        //Toogle
        invertedXToggle.onValueChanged.AddListener(SetInvertedXCamera);
        invertedYToggle.onValueChanged.AddListener(SetInvertedYCamera);
        fullscreenToggle.onValueChanged.AddListener(SetFullScreenBool);

        //Music
        musicSlider.onValueChanged.AddListener(ChangeMusicVolumeSlider);
        soundSlider.onValueChanged.AddListener(ChangeSoundVolumeSlider);

        musicInput.onValueChanged.AddListener(ChangeMusicVolumeInput);
        soundInput.onValueChanged.AddListener(ChangeSoundVolumeInput);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ButtonClick()
    {
        animManager.SetBool("isIdle", false);
        animManager.SetBool("isComeback", false);
        animManager.SetBool("isSelected", true);
    }
    public void OkButtonClick()
    {
        animManager.SetBool("isIdle", false);
        animManager.SetBool("isSelected", false);
        animManager.SetBool("isComeback", true);
    }

    public void LoadSettings()
    {
        //Toogle Camera
        invertedXToggle.isOn = PlayerPrefs.GetInt(InvertedX, 0) == 1 ? true : false;
        invertedYToggle.isOn = PlayerPrefs.GetInt(InvertedY, 0) == 1 ? true : false;
        SetInvertedXCamera(invertedXToggle.isOn);
        SetInvertedYCamera(invertedYToggle.isOn);

        //Set Mouse Sliders Values
        sensitivityXSlider.value = PlayerPrefs.GetInt(sensXTag, defaultVelX);
        sensitivityYSlider.value = PlayerPrefs.GetInt(sensYTag, defaultVelY);

        //Set Mouse Input Values
        sensitivityXInput.text = PlayerPrefs.GetInt(sensXTag, defaultVelX).ToString();
        sensitivityYInput.text = PlayerPrefs.GetInt(sensYTag, defaultVelY).ToString();


        //Set Audio Values
        musicSlider.value = PlayerPrefs.GetFloat(musicTag, 50);
        soundSlider.value = PlayerPrefs.GetFloat(soundTag, 50);

        //Set Audio Input Values
        musicInput.text = PlayerPrefs.GetFloat(musicTag, 50).ToString();
        soundInput.text = PlayerPrefs.GetFloat(soundTag, 50).ToString();

        //Set full screen
        isFullscreen = PlayerPrefs.GetInt("FullScreen", 0) == 1;
        fullscreenToggle.isOn = isFullscreen;      
    }

    #region Camera options
    public void SetInvertedXCamera(bool _value) //Toogle
    {
        PlayerPrefs.SetInt(InvertedX, _value ? 1 : 0);
        if (settingsCameraPlayer)
        {
            settingsCameraPlayer.SetInvertedX(_value);
        }        
    }
    public void SetInvertedYCamera(bool _value)
    {
        PlayerPrefs.SetInt(InvertedY, _value ? 1 : 0);
        if (settingsCameraPlayer)
        {
            settingsCameraPlayer.SetInvertedY(_value);
        }
    } //Toogle

    //Y axis
    public void ChangeSensitivityYSlider(float _value) //Slider
    {
        float valueToSlide = ((float)Mathf.Round(_value * 100f) / 100f);
        
        PlayerPrefs.SetFloat(sensYTag, _value);
        sensitivityYInput.text = _value.ToString();
        
        if (settingsCameraPlayer)
        {
            settingsCameraPlayer.ChangeVelocityY(_value);
        }
    }
    public void ChangeSensitivityYInput(string _value)
    {
        if (float.TryParse(_value, out float sensitivityValue))
        {
            if (sensitivityValue > sensitivityYSlider.maxValue)
            {
                sensitivityValue = sensitivityYSlider.maxValue;
                sensitivityYInput.text = sensitivityYSlider.maxValue.ToString();
            }
            else if (sensitivityValue < sensitivityYSlider.minValue)
            {
                sensitivityValue = sensitivityYSlider.minValue;
                sensitivityYInput.text = sensitivityYSlider.minValue.ToString();
            }
            sensitivityYSlider.value = sensitivityValue;
            PlayerPrefs.SetInt(sensYTag, (int) sensitivityValue);
            if (settingsCameraPlayer)
            {
                settingsCameraPlayer.ChangeVelocityY(sensitivityValue);
            }
        }
    } //Input Text
    
    //X axis
    public void ChangeSensitivityXSlider(float _value)
    {
        PlayerPrefs.SetInt(sensXTag, (int)_value);
        sensitivityXInput.text = _value.ToString();
        if (settingsCameraPlayer)
        {
            settingsCameraPlayer.ChangeVelocityX(_value);
        }
    } //Slider
    public void ChangeSensitivityXInput(string _value)
    {
        if (float.TryParse(_value, out float sensitivityValue))
        {
            if (sensitivityValue > sensitivityXSlider.maxValue)
            {
                sensitivityValue = sensitivityXSlider.maxValue;
                sensitivityXInput.text = sensitivityXSlider.maxValue.ToString();
            }
            else if (sensitivityValue < sensitivityXSlider.minValue)
            {
                sensitivityValue = sensitivityXSlider.minValue;
                sensitivityXInput.text = sensitivityXSlider.minValue.ToString();
            }
            sensitivityXSlider.value = sensitivityValue;
            PlayerPrefs.SetInt(sensXTag, (int) sensitivityValue);
            if (settingsCameraPlayer)
            {
                settingsCameraPlayer.ChangeVelocityX(sensitivityValue);
            }
        }
    } //Input text

    #endregion

    #region Screen options

    public void SetFullScreenBool(bool _value)
    {
        PlayerPrefs.SetInt("FullScreen", _value ? 1 : 0);
        Screen.fullScreen = _value;
        //if (_value)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = true;
        //}
        //else
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
    }
    

    #endregion

    #region Audio Options
    public void ChangeSoundVolumeInput(string _value)
    {
        if(float.TryParse(_value, out float soundVolume))
        {
            if (soundVolume > 100)
            {
                soundVolume = 100;
                soundInput.text = "100";
            }
            else if (soundVolume < 0)
            {
                soundVolume = 0;
                soundInput.text = "0";
            }

            //soundMixer.SetFloat(soundTag, Mathf.Log10(soundVolume) * 20);
            PlayerPrefs.SetFloat(soundTag, soundVolume);
            soundSlider.value = soundVolume;
            print(soundVolume);
        }
    }
    public void ChangeMusicVolumeInput(string _value)
    {
        if (float.TryParse(_value, out float musicVolume))
        {
            if(musicVolume > 100)
            {
                musicVolume = 100;
                musicInput.text = "100";
            }
            else if(musicVolume < 0)
            {
                musicVolume = 0;
                musicInput.text = "0";
            }

            
            
            //musicMixer.SetFloat(musicTag, Mathf.Log10(musicVolume) * 20);
            PlayerPrefs.SetFloat(musicTag, musicVolume);
            musicSlider.value = musicVolume;
        }
    }
    
    //Wise
    public void ChangeSoundVolumeSlider(float _value)
    {
        //soundMixer.SetFloat(soundTag, Mathf.Log10(soundSlider.value) * 20);
        PlayerPrefs.SetFloat(soundTag, soundSlider.value);
        soundInput.text = _value.ToString();
        AkSoundEngine.SetRTPCValue("SFX_Volume", soundSlider.value);
    }
    public void ChangeMusicVolumeSlider(float _value)
    {
        //musicMixer.SetFloat(musicTag, Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat(musicTag, musicSlider.value);
        musicInput.text = _value.ToString();
        AkSoundEngine.SetRTPCValue("Music_Volume", musicSlider.value);
    }

    #endregion

    #region Getters
    public bool GetFullScreen()
    {
        return Screen.fullScreen;
    }
    public bool GetInvertedXCamera()
    {
        return invertedX;
    }
    public bool GetInvertedYCamera()
    {
        return invertedY;
    }
    #endregion

}
