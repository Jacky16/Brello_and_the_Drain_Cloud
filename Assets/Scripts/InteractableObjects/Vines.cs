using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vines : Interactable
{
    [SerializeField] float timeToDestroy;
    [SerializeField] GameObject smokeParticles;
    ParticleSystem fire;
    Color initColor;


    protected override void Start()
    {
        base.Start();
        fire = transform.GetChild(0).GetComponent<ParticleSystem>();
        fire.gameObject.SetActive(false);
    }

    public override void Interact()
    {
        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        fire.gameObject.SetActive(true);
        AkSoundEngine.PostEvent("Fire_Pyra", WwiseManager.instance.gameObject);

        initColor = transform.GetChild(1).GetComponent<MeshRenderer>().material.color;
        transform.GetChild(1).GetComponent<MeshRenderer>().material.DOColor(Color.black, timeToDestroy);

        yield return new WaitForSeconds(timeToDestroy);

        Instantiate(smokeParticles, transform.position, Quaternion.identity);

        pyra.isInteracting = false;

        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = initColor;
        Destroy(gameObject);
    }

    public override void ResetInter()
    {
        StopAllCoroutines();
        ResetAll();
    }

    protected override void ResetAll()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        pyra.isInteracting = false;
        fire.gameObject.SetActive(false);
    }
}
