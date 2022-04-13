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
    PlayerController player;
    Tween currentTween;
    private void Start()
    {
        normalMovement = true;
        spotToFollow = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pyra = GameObject.FindGameObjectWithTag("Pyra");
    }
    // Update is called once per frame
    void Update()
    {
        if (normalMovement && Vector3.Distance(transform.position, spotToFollow.position) >= 0f)
        {
            currentTween = transform.DOMove(spotToFollow.position, 1f);
        }
    }

    public void posToFinish(Vector3 pos)
    {
        currentTween.Kill();
        normalMovement = false;

        transform.DOMove(pos, 0.75f).OnComplete(() =>
        {
            Destroy(gameObject);
            pyra.transform.DOScale(1f, 0.25f).OnComplete(()=>
            {
                //Reactivar el agent para que se recoloque en la navmesh.
                pyra.GetComponent<NavMeshAgent>().enabled = false;
                pyra.GetComponent<NavMeshAgent>().enabled = true;
            });
        });
    }

    public void FinishInWater(Vector3 pos)
    {
        DOTween.Kill(this);

        Vector3 offsetedPos = new Vector3(pos.x, pos.y+0.005f, pos.z);

        normalMovement = false;

        StartCoroutine(Wait());

        transform.DOLocalMove(offsetedPos, 1f).OnComplete(() =>
        {
            transform.DOScale(0f, 1f).OnComplete(() =>
            {
                Destroy(gameObject);         
            });

        }).OnUpdate(() =>
        {
            transform.localScale = Vector3.one;
        });
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.75f);
        pyra.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
}
