using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] GameObject steamlingPrefab;

    [SerializeField] GameObject smokePrefab;

    
    private void OnCollisionEnter(Collision collision)
    {
        //compare tag wall
        if (collision.gameObject.tag == "Wall")
        {
            //instantiate steam link
            GameObject go = Instantiate(steamlingPrefab, transform.position, Quaternion.identity);
            go.GetComponent<SteamlingAI>().SetBigRadius();

            Instantiate(smokePrefab, transform.position, Quaternion.identity);
            //destroy gameobject
            Destroy(gameObject);
        }        
    }
}
