using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FinalTrigger : MonoBehaviour
{
    [SerializeField] PlayableDirector playable;
    [SerializeField] Animator animator;
    PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Appear");
            playable.Play();
            controller.BlockMovement();
        }
    }
}
