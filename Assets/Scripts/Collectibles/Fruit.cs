using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Drop()
    {
        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent(out BrelloHealth brelloHealth))
        {
            brelloHealth.DoHeal(1);
            Destroy(gameObject);
        }
    }
}
