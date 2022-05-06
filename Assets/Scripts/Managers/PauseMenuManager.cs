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

    private void Start()
    {
        //PauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPause = !isPause;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = isPause;
            if (isPause)
            {
                Time.timeScale = 0;
                PauseMenu.SetActive(true);


            }
            else
            {
                Time.timeScale = 1;
                PauseMenu.SetActive(false);
            }                
        }
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