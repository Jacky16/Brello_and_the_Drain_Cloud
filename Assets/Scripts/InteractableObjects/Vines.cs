using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vines : Interactable
{
    [SerializeField] float timeToDestroy;
    ParticleSystem fire;

    protected override void Start()
    {
        base.Start();
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        fire = GetComponentInChildren<ParticleSystem>();
        fire.gameObject.SetActive(false);
    }

    public override void Interact()
    {
        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        fire.gameObject.SetActive(true);

        AkSoundEngine.PostEvent("Fire_Pyra", WwiseManager.instance.gameObject);

        yield return new WaitForSeconds(timeToDestroy);

        pyra.isInteracting = false;
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
