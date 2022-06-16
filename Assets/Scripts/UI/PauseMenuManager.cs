using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject canvasPause;
    [SerializeField] GameObject canvasOptions;
    [SerializeField] BackgroundMusic bm;
    bool isPause = false;
    PlayerCam playerCam;

    const string soundTag = "SoundVol";
    const string musicTag = "MusicVol";
    const string sensXTag = "SensX";
    const string sensYTag = "SensY";

    private void Awake()
    {
        playerCam = FindObjectOfType<PlayerCam>();
    }
    private void Start()
    {
        canvasPause.SetActive(false);
        canvasOptions.SetActive(false);
    }
  
    public void Pause()
    {
        isPause = !isPause;
        canvasPause.SetActive(isPause);
        canvasOptions.SetActive(isPause);

        if (isPause)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
            Resume();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canvasOptions.SetActive(false);
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