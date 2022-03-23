using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] GameObject steamLinkPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(steamLinkPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
}
