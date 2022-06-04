using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPilar : MonoBehaviour
{
    [SerializeField] GameObject particlesToSpawn;
    [SerializeField] Transform posToSpawnParticles;

    public void SpawnParticles()
    {
        Instantiate(particlesToSpawn, posToSpawnParticles.position, Quaternion.identity);
    }
}
