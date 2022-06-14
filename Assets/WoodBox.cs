using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBox : Health
{
    [SerializeField] GameObject destroyParticles;

    protected override void onDeath()
    {
        Instantiate(destroyParticles, transform.position, Quaternion.identity).transform.localScale = Vector3.one * 2;
        Destroy(gameObject);
    }
}
