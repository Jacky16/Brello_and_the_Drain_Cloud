using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindowScript : MonoBehaviour
{
    public GameObject CreditsScreen;
    public GameObject CreditsBack;
    public GameObject GlobalCredits;

    public float SecondsToWait;

    public bool canClick;

    Animator animExitCreditsBack;
    Animator animExitCredits;

    void Awake()
    {
        animExitCredits = CreditsScreen.GetComponent<Animator>();
        animExitCreditsBack = CreditsBack.GetComponent<Animator>();
    }

    void Start()
    {
        canClick = false;
        StartCoroutine(areCreditsIn());
        canClick = true;
    }

    public void clickOK()
    {
        if (canClick == true)
        {
            animExitCredits.SetBool("credOut", true);
            animExitCreditsBack.SetBool("credOut", true);
            canClick = false;
            StartCoroutine(areCreditsOut());
        }
    }

    IEnumerator areCreditsOut()
    {
        yield return new WaitForSeconds(SecondsToWait);
        canClick = true;
        GlobalCredits.SetActive(false);
    }

    IEnumerator areCreditsIn()
    {
        yield return new WaitForSeconds(SecondsToWait);
    }
}
