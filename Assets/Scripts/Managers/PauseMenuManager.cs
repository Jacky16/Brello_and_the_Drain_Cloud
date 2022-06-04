using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    bool isPause = false;

    [SerializeField] BackgroundMusic bm;
    PlayerCam playerCam;

    const string soundTag = "SoundVol";
    const string musicTag = "MusicVol";
    const string sensXTag = "SensX";
    const string sensYTag = "SensY";

    private void Awake()
    {
        playerCam = FindObjectOfType<PlayerCam>();
        LoadSettings();
    }
    private void Start()
    {
        PauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            Time.timeScale = isPause ? 0 : 1;
        }
    }
    void LoadSettings()
    {
        InitCameraVelocity();
        InitSound();
    }
    public void InitCameraVelocity()
    {
        if (playerCam)
        {
            playerCam.ChangeVelocityY(PlayerPrefs.GetFloat(sensYTag, playerCam.GetVelocityY()));
            playerCam.ChangeVelocityX(PlayerPrefs.GetFloat(sensXTag, playerCam.GetVelocityX()));
        }
    }
  

    public void InitSound()
    {
        AkSoundEngine.SetRTPCValue("SFX_Volume", PlayerPrefs.GetFloat(soundTag, 50));
        AkSoundEngine.SetRTPCValue("Music_Volume", PlayerPrefs.GetFloat(musicTag, 50));

    }
  
    public void Pause()
    {
        isPause = !isPause;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = isPause;
        PauseMenu.SetActive(isPause);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPause = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = isPause;
    }

    public void Restart()
    {
        //Reload Current Scene
        Time.timeScale = 1;
        bm.StopMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        bm.StopMusic();
        SceneManager.LoadScene("MainMenu");
    }
}    