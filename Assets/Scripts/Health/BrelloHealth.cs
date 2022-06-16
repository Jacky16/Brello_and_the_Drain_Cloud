using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public sealed class BrelloHealth : Health
{
    [Header("Life Sprite Variables")]
    [SerializeField] Sprite[] healthImages;
    [SerializeField] Image currentHealthImage;
    [SerializeField] float timeInHUD;
    [SerializeField] GameObject damageParticles;
    [SerializeField] Transform spawnPoint;

    [SerializeField] Image canvasFade;
    float lastLifeChange;
    bool lifeChanged;

    [SerializeField] BackgroundMusic bm;

    private Cinemachine.CinemachineImpulseSource cameraShake;

    // Start is called before the first frame update
    private void Awake()
    {
        cameraShake = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }
    protected override void Start()
    {
        base.Start();        
        lastLifeChange = 0f;
        lifeChanged = false;
        //Fade canvas
        canvasFade.DOFade(0f, 1f);
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
        if (damageParticles && spawnPoint != null)
        {
            Instantiate(damageParticles, spawnPoint.position, Quaternion.identity);
        }
        lastLifeChange = 0f;
        currentHealthImage.sprite = healthImages[currLife];
        cameraShake.GenerateImpulse();
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
        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            bm.StopMusic();
            ReloadSceneBoss();
        }
        else
        {                        
            StartCoroutine(Reappear());
        }
    }

    void ReloadSceneBoss()
    {
        canvasFade.DOFade(1, 2).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
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
