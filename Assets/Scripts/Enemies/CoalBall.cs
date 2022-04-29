using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CoalBall : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float impulseForce;
    [SerializeField] private float impulseDuration;
    [SerializeField] private int damage;
    [SerializeField] private float altitudeOffset;
    private Vector3 offset;
    private float speed = 1300f;

    Transform player;
    void Start()
    {
        offset = new Vector3(0, altitudeOffset, 0);
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb.DOJump(player.position - offset, impulseForce, 1, impulseDuration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out BrelloHealth playerHealth))
        {
            playerHealth.DoDamage(damage);
        }

        //Reproducir sonido
        //Spawn particulas
        Destroy(gameObject);
    }
}
