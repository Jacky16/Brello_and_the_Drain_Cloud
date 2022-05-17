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
    

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;
    [SerializeField] TMP_InputField soundInput;
    [SerializeField] TMP_InputField musicInput;

    [SerializeField] Slider sensitivityXSlider;
    [SerializeField] Slider sensitivityYSlider;
    [SerializeField] TMP_InputField sensitivityXInput;
    [SerializeField] TMP_InputField sensitivityYInput;

    [SerializeField] const string soundTag = "SoundVol";
    [SerializeField] const string musicTag = "MusicVol";
    [SerializeField] const string sensXTag = "SensX";
    [SerializeField] const string sensYTag = "SensY";

    [SerializeField] BackgroundMusic bm;


    [Header("MAIN MENU ANIMATIONS")]

    public GameObject MenuAnimManager;
    Animator animManager;
    PlayerCam playerCam;
    private void Awake()
    {
        playerCam = GameObject.FindGameObjectWithTag("CamPlayer").GetComponent<PlayerCam>();
        if (!animManager)
        {
            animManager = MenuAnimManager.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (animManager)
        {           
            animManager.SetBool("isIdle", true);
        }
    }
    public void StartGame()
    {
        bm.StopMusic();
        SceneManager.LoadScene(1);
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

    #region Camera options
    public void SetInvertedXCamera(bool _value)
    {
        playerCam.ChangeInvertX(_value);
    }
    public void SetInvertedYCamera(bool _value)
    {
        playerCam.ChangeInvertY(_value);
    }

    //Y axis
    public void ChangeSensitivityYSlider(float _value)
    {
        PlayerPrefs.SetFloat(sensYTag, _value);
        sensitivityYInput.text = ((float)Mathf.Round(_value * 100f) / 100f).ToString();
        playerCam.ChangeVelocityY(_value);
    }
    public void ChangeSensitivityYInput(string _value)
    {
        if (float.TryParse(_value, out float sensitivityValue))
        {
            if (sensitivityValue > 1.25f)
            {
                sensitivityValue = 1.25f;
                sensitivityYInput.text = "1.25";
            }
            else if (sensitivityValue < 0.7f)
            {
                sensitivityValue = 0.7f;
                sensitivityYInput.text = "0.7";
            }

            sensitivityYSlider.value = sensitivityValue;
            PlayerPrefs.SetFloat(sensYTag, sensitivityValue);
        }
    }
    
    //X axis
    public void ChangeSensitivityXSlider(float _value)
    {
        PlayerPrefs.SetInt(sensXTag, (int)_value);
        sensitivityXInput.text = _value.ToString();
        playerCam.ChangeVelocityX(_value);
    }
    public void ChangeSensitivityXInput(string _value)
    {
        if (float.TryParse(_value, out float sensitivityValue))
        {
            if (sensitivityValue > 150)
            {
                sensitivityValue = 150;
                sensitivityXInput.text = "150";
            }
            else if (sensitivityValue < 100)
            {
                sensitivityValue = 100;
                sensitivityXInput.text = "100";
            }

            sensitivityXSlider.value = sensitivityValue;
            PlayerPrefs.SetFloat(sensXTag, sensitivityValue);
        }
    }

    #endregion

    #region Screen options

    public void SetFullScreenBool(bool _value)
    {
        Screen.fullScreen = _value;
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
