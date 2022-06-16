using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vines : Interactable
{
    [SerializeField] float timeToDestroy;
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

        initColor = transform.GetComponent<MeshRenderer>().material.color;
        transform.GetComponent<MeshRenderer>().material.DOColor(Color.black, timeToDestroy);

        yield return new WaitForSeconds(timeToDestroy);

        pyra.isInteracting = false;
        pyra.RefreshDetectedObject();
        transform.GetComponent<MeshRenderer>().material.color = initColor;
        Destroy(gameObject);
    }

    public override void ResetInter()
    {
        StopAllCoroutines();
        ResetAll();
    }

    protected override void ResetAll()
    {
        transform.GetComponent<MeshRenderer>().material.color = initColor;
        pyra.isInteracting = false;
        fire.gameObject.SetActive(false);
    }
}
