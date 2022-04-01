using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafPile : Interactable
{

    [Range(0, 100)]
    [SerializeField] float coinPercent;

    [SerializeField] float timeToDestroy;

    [SerializeField] GameObject coin;
    ParticleSystem fire;

    float rand;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rand = Random.Range(0f, 100f);
        fire = GetComponentInChildren<ParticleSystem>();
        fire.gameObject.SetActive(false);
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    public override void Interact()
    {
        StartCoroutine(Burn());
    }

    private IEnumerator Burn()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        fire.gameObject.SetActive(true);

        yield return new WaitForSeconds(timeToDestroy);

        if(rand <= coinPercent)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
