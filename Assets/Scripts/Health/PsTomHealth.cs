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
    [SerializeField] GameObject[] gameObjectsToDisable;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        psTom = GetComponent<PsTom>();
    }
    
    protected override void onDamage()
    {
        base.onDamage();
        float percent = (float)currLife / (float)maxLife;     
        healthBar.DOFillAmount(percent, 0.5f);   
        psTom.ChangePhase(currLife);
        anim.SetTrigger("Hit");
    }
    protected override void onDeath()
    {
        base.onDeath();
        timeline.Play();
        
        DisableAllObjects();
        anim.SetTrigger("Death");
        
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene("MainMenu");
    }
    void DisableAllObjects()
    {
        foreach (GameObject go in gameObjectsToDisable)
        {
            go.SetActive(false);
        }
    }
}
