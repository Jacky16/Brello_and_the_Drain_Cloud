using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
   [SerializeField] GameObject PauseMenu; 
    bool isPause;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPause = !isPause;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = isPause;
            PauseMenu.SetActive(isPause);
            if (isPause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}