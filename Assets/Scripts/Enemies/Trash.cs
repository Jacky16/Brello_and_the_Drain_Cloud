using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] GameObject steamLinkPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        //compare tag wall
        if (collision.gameObject.tag == "Wall")
        {
            //instantiate steam link
            Instantiate(steamLinkPrefab, transform.position, Quaternion.identity);
            //destroy gameobject
            Destroy(gameObject);
        }        
    }
}
