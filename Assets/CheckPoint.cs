using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    bool isSaved;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        isSaved = false;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BrelloHealth brello) && !isSaved)
        {
            brello.SetSpawnPoint(transform.GetChild(0));
            anim.SetTrigger("Save");

            //Poner sonidito del checkpoint

            isSaved = true;
        }
    }
}
