using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenScript : MonoBehaviour
{
    [Header("Sequence Images")]
    public GameObject SeqImg00;
    public GameObject SeqImg01;
    public GameObject SeqImg02;
    public GameObject SeqImg03;
    public GameObject SeqImg04;
    public GameObject SeqImg05;
    public GameObject SeqImg06;

    [Header("Animators")]
    Animator animSeqImg00;
    Animator animSeqImg01;
    Animator animSeqImg02;
    Animator animSeqImg03;
    Animator animSeqImg04;
    Animator animSeqImg05;
    Animator animSeqImg06;

    [Header("Timer")]
    [Range(0,1)]
    public float SecondsBetweenAnims;

    [Header("Comprobaciones")]
    public bool isAnimFinished;

    [Header("References")]
    public GameObject MainMenu;
    public GameObject SplashScreen;

    void Awake()
    {
        animSeqImg00 = SeqImg00.GetComponent<Animator>();
        animSeqImg01 = SeqImg01.GetComponent<Animator>();
        animSeqImg02 = SeqImg02.GetComponent<Animator>();
        animSeqImg03 = SeqImg03.GetComponent<Animator>();
        animSeqImg04 = SeqImg04.GetComponent<Animator>();
        animSeqImg05 = SeqImg05.GetComponent<Animator>();
        animSeqImg06 = SeqImg06.GetComponent<Animator>();    
    }

    void Start()
    {
        isAnimFinished = false;

        Invoke("Seq01active", SecondsBetweenAnims);
        Invoke("Seq02active", SecondsBetweenAnims * 2);
        Invoke("Seq03active", SecondsBetweenAnims * 3);
        Invoke("Seq04active", SecondsBetweenAnims * 4);
        Invoke("Seq05active", SecondsBetweenAnims * 5);
        Invoke("Seq06active", SecondsBetweenAnims * 6);
    }

    void Seq01active()
    {
        SeqImg01.SetActive(true);
    }

    void Seq02active()
    {
        SeqImg02.SetActive(true);
    }

    void Seq03active()
    {
        SeqImg03.SetActive(true);
    }

    void Seq04active()
    {
        SeqImg04.SetActive(true);
    }
    void Seq05active()
    {
        SeqImg05.SetActive(true);
    }

    void Seq06active()
    {
        SeqImg06.SetActive(true);
        StartCoroutine(isFinishedAnim());
    }

    IEnumerator isFinishedAnim()
    {
        yield return new WaitForSeconds(1);
        isAnimFinished = true;
    }

    IEnumerator isSequenceFinished()
    {
        yield return new WaitForSeconds(1);
        SplashScreen.SetActive(false);
    }

    void NextStage01()
    {
        if (isAnimFinished == true)
        {
            if (Input.anyKey)
            {
                animSeqImg00.SetBool("NextState", true);
                animSeqImg01.SetBool("NextState", true);
                animSeqImg02.SetBool("NextState", true);
                animSeqImg03.SetBool("NextState", true);
                animSeqImg04.SetBool("NextState", true);
                animSeqImg05.SetBool("NextState", true);
                animSeqImg06.SetBool("NextState", true);
                StartCoroutine(isSequenceFinished());
            }
        }
    }

    void Update()
    {
        NextStage01();
    }
}
