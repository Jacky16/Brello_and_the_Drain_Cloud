using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    bool invertedX = false;
    bool invertedY = true;
    bool fullScreen = true;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider sensitivityXSlider;
    [SerializeField] Slider sensitivityYSlider;
    [SerializeField] AudioMixer soundMixer;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] TMP_InputField soundInput;
    [SerializeField] TMP_InputField musicInput;
    [SerializeField] TMP_InputField sensitivityXInput;
    [SerializeField] TMP_InputField sensitivityYInput;

    [SerializeField] const string soundTag = "SoundVol";
    [SerializeField] const string musicTag = "MusicVol";
    [SerializeField] const string sensXTag = "SensX";
    [SerializeField] const string sensYTag = "SensY";

    public void SetInvertedXCamera()
    {
        invertedX = !invertedX;
    }
    public bool GetInvertedXCamera()
    {
        return invertedX;
    }
    public void SetInvertedYCamera()
    {
        invertedY = !invertedY;
    }
    public bool GetInvertedYCamera()
    {
        return invertedY;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SetFullScreenBool()
    {
        fullScreen = !fullScreen;
        SetFullScreen(fullScreen);
    }
    private void SetFullScreen(bool value)
    {
        Screen.fullScreen = !value;
    }
    public bool GetFullScreen()
    {
        return Screen.fullScreen;
    }
    public void ChangeSensitivityXSlider()
    {
        PlayerPrefs.SetInt(sensXTag, (int)sensitivityXSlider.value);
        sensitivityXInput.text = sensitivityXSlider.value.ToString();
    }
    public void ChangeSensitivityXInput()
    {
        if (float.TryParse(sensitivityXInput.text, out float sensitivityValue))
        {
            if (sensitivityValue > 150)
            {
                sensitivityValue = 150;
                sensitivityXInput.text = "150";
            }
            else if(sensitivityValue < 100)
            {
                sensitivityValue = 100;
                sensitivityXInput.text = "100";
            }

            PlayerPrefs.SetFloat(sensXTag, sensitivityValue);
        }
    }
    public void ChangeSensitivityYSlider()
    {
        PlayerPrefs.SetFloat(sensYTag, sensitivityYSlider.value);
        sensitivityYInput.text = ((float)Mathf.Round(sensitivityYSlider.value * 100f) / 100f).ToString();
    }
    public void ChangeSensitivityYInput()
    {
        if (float.TryParse(sensitivityYInput.text, out float sensitivityValue))
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

            PlayerPrefs.SetFloat(sensYTag, sensitivityValue);
        }
    }
    public void ChangeSoundVolumeInput()
    {
        if(float.TryParse(soundInput.text, out float soundVolume))
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

            soundMixer.SetFloat(soundTag, Mathf.Log10(soundVolume) * 20);
            PlayerPrefs.SetFloat(soundTag, soundVolume);
            soundSlider.value = soundVolume;
        }
    }
    public void ChangeMusicVolumeInput()
    {
        if (float.TryParse(musicInput.text, out float musicVolume))
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

            musicMixer.SetFloat(musicTag, Mathf.Log10(musicVolume) * 20);
            PlayerPrefs.SetFloat(musicTag, musicVolume);
            musicSlider.value = musicVolume;
        }
    }
    public void ChangeSoundVolumeSlider()
    {
        soundMixer.SetFloat(soundTag, Mathf.Log10(soundSlider.value) * 20);
        PlayerPrefs.SetFloat(soundTag, soundSlider.value);
        soundInput.text = soundSlider.value.ToString();
    }
    public void ChangeMusicVolumeSlider()
    {
        musicMixer.SetFloat(musicTag, Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat(musicTag, musicSlider.value);
        musicInput.text = musicSlider.value.ToString();
    }
}
