using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hacks : MonoBehaviour
{
    Transform zone1;
    Transform zone2;
    Transform zone3;
    Transform zone4;
    GameObject player;
    NavMeshAgent pyra;
    // Start is called before the first frame update
    void Start()
    {
        zone2 = transform.GetChild(0).transform;
        zone3 = transform.GetChild(1).transform;
        zone4 = transform.GetChild(2).transform;
        zone1 = transform.GetChild(3).transform;
        player = GameObject.FindGameObjectWithTag("Player");
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.transform.position = zone1.transform.position;
            pyra.Warp(player.transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.transform.position = zone2.transform.position;
            pyra.Warp(player.transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.transform.position = zone3.transform.position;
            pyra.Warp(player.transform.position);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            player.transform.position = zone4.transform.position;
            pyra.Warp(player.transform.position);
        }
    }
}
