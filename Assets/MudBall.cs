using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MudBall : MonoBehaviour
{
    Transform player;
    Rigidbody rb;
    [SerializeField] float jumpForce;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.DOJump(player.position, jumpForce, 1, 1.5f);
    }

}
