using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject OptionsMenu;
    public GameObject AnimManager;
    public GameObject BGanim;

    Animator BG;
    Animator Options;
    Animator Manager;

    public float animDuration;

    void Start()
    {
        BG = BGanim.GetComponent<Animator>();
        Options = OptionsMenu.GetComponent<Animator>();
        Manager = AnimManager.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PauseMenu.gameObject.activeSelf == false)
            {
                PauseMenu.SetActive(true);

                StartCoroutine(AnimTimeIn());

                BG.SetBool("isIn", true);
                BG.SetBool("isOut", false);

                Options.SetBool("credIn", true);
                Options.SetBool("credOut", false);

                Manager.SetBool("pauseIsIn", true);
                Manager.SetBool("pauseIsOut", false);

            }

            else
            {
                BG.SetBool("isIn", false);
                BG.SetBool("isOut", true);

                Options.SetBool("credIn", false);
                Options.SetBool("credOut", true);

                Manager.SetBool("pauseIsIn", false);
                Manager.SetBool("pauseIsOut", true);

                StartCoroutine(AnimTimeOut());
            }
        }
    }

    IEnumerator AnimTimeOut()
    {
        yield return new WaitForSeconds(animDuration);
        PauseMenu.SetActive(false);
    }

    IEnumerator AnimTimeIn()
    {
        yield return new WaitForSeconds(animDuration);
    }
}
