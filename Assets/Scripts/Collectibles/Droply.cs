using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droply : MonoBehaviour
{
    float initY;
    private void Start()
    {
        initY = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, initY + Mathf.Sin(Time.time * 10) * 0.25f + 0.25f, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Brello_Collectables collectables))
        {
            collectables.AddDroply();
            Destroy(gameObject);
        }
    }
}
