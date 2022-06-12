using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] GameObject healParticles;
    Animator anim;
    Rigidbody rb;
    public bool canBePickedUp;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        canBePickedUp = false;

        StartCoroutine(JiggleFruit(Random.Range(0f, 1f)));
    }


    IEnumerator JiggleFruit(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Entro");
        anim.SetTrigger("Start");
    }
    public void Drop()
    {
        anim.SetTrigger("Drop");
        rb.useGravity = true;
        canBePickedUp = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent(out BrelloHealth brelloHealth))
        {
            if (canBePickedUp)
            {
                brelloHealth.DoHeal(1);
                Instantiate(healParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
