using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PsTomHealth : Health
{
    PsTom psTom;
    Animator anim;
    [SerializeField] PlayableDirector timeline;
    [SerializeField] Image healthBar;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        psTom = GetComponent<PsTom>();
        //isInmune = true;
    }
    
    protected override void onDamage()
    {
        base.onDamage();
        
        healthBar.DOFillAmount((float)currLife / (float)maxLife, 0.5f);   
        psTom.ChangePhase(currLife);
    }
    protected override void onDeath()
    {
        base.onDeath();
        timeline.Play();
        //Disable game object player from find tag
        GameObject.FindGameObjectWithTag("Player").SetActive(false);
        GetComponent<Animator>().SetTrigger("Death");
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene("MainMenu");
    }
}
