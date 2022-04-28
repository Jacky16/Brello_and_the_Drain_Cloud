using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SteamlingHealth : Health
{
    [SerializeField] private GameObject cloudMask;
    [SerializeField] GameObject starParticles;
    [SerializeField] GameObject cloudParticles;
    [SerializeField] Transform posToSpawnParticles;
    private MeshRenderer rendererSteamling;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rendererSteamling = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    protected override void onDamage()
    {   
        if(currLife == 1)
        {
            //Si tiene animacion de daño
            //animator.SetTrigger("Damage");
            Instantiate(cloudParticles, posToSpawnParticles.position, Quaternion.identity);
            cloudMask.SetActive(false);
            
        }
        rendererSteamling.material.SetColor("_MainColor", Color.red);
        StartCoroutine(ResetColor());

        //AkSoundEngine.PostEvent("Hurt_Steamling", WwiseManager.instance.gameObject);
    }

    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.4f);
        rendererSteamling.material.SetColor("_MainColor", Color.white);
    }
    protected override void onDeath()
    {
        if (combatManager.enemyList.Contains(gameObject))
        {
            combatManager.enemyList.Remove(gameObject);
        }

        Instantiate(starParticles, posToSpawnParticles.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
