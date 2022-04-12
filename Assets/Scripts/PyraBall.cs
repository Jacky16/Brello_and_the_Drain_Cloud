using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class PyraBall : MonoBehaviour
{
    Transform spotToFollow;
    [SerializeField] float speed;
    Vector3 correctedPos;
    bool normalMovement;
    GameObject pyra;
    private void Start()
    {
        normalMovement = true;
        spotToFollow = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        pyra = GameObject.FindGameObjectWithTag("Pyra");
    }
    // Update is called once per frame
    void Update()
    {
        if (normalMovement)
        {
            transform.DOMove(spotToFollow.position, 1f);
        }
    }

    public void posToFinish(Vector3 pos)
    {
        DOTween.KillAll();
        normalMovement = false;

        transform.DOMove(pos, 0.25f).OnComplete(() =>
        {
            transform.DOScale(0f, 0.25f).OnComplete(() =>
            {
                Destroy(gameObject);
            });

            pyra.transform.localScale = Vector3.one;

            //Reactivar el agent para que se recoloque en la navmesh.
            pyra.GetComponent<NavMeshAgent>().enabled = false;
            pyra.GetComponent<NavMeshAgent>().enabled = true;
        });
    }
}
