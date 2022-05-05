using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.AI;

public sealed class BrelloHealth : Health
{
    [Header("Life Sprite Variables")]
    [SerializeField] Sprite[] healthImages;
    [SerializeField] Image currentHealthImage;
    [SerializeField] float timeInHUD;
    [SerializeField] GameObject damageParticles;
    [SerializeField] Transform spawnPoint;

    Animator imageAnimator;
    float lastLifeChange;
    bool lifeChanged;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //imageAnimator = currentImage.GetComponent<Animator>();
        lastLifeChange = 0f;
        lifeChanged = false;
    }

    private void Update()
    {
        if (lifeChanged)
        {
            lastLifeChange += Time.deltaTime;
            if(lastLifeChange >= timeInHUD)
            {
                if (currLife > 2)
                {   
                    
                    lastLifeChange = 0f;
                    lifeChanged = false;
                    //imageAnimator.SetTrigger("Disappear");
                }
            }
        }
    }

    protected override void onDamage()
    {
        //Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        lifeChanged = true;
        //imageAnimator.SetTrigger("Appear");
        if (damageParticles)
        {
            Instantiate(damageParticles, spawnPoint.position, Quaternion.identity);
        }
        lastLifeChange = 0f;
        currentHealthImage.sprite = healthImages[currLife];
    }

    protected override void onHeal()
    {

        lifeChanged = true;
        //imageAnimator.SetTrigger("Appear");
        lastLifeChange = 0f;
        currentHealthImage.sprite = healthImages[currLife];
    }

    protected override void onDeath()
    {
        animator.SetTrigger("Death");
        StartCoroutine(Reappear());
    }

    private IEnumerator Reappear()
    {
        yield return new WaitForSeconds(timeToReappear);
        ResetStats();
    }

    protected void ResetStats()
    {
        currLife = maxLife;
        isInmune = false;
        currentHealthImage.sprite = healthImages[currLife];
        transform.position = spawnPoint.position;

        GameObject.FindGameObjectWithTag("Pyra").GetComponent<NavMeshAgent>().enabled = false;
        GameObject.FindGameObjectWithTag("Pyra").transform.position = spawnPoint.position;
        GameObject.FindGameObjectWithTag("Pyra").GetComponent<NavMeshAgent>().enabled = true;
    }
}
