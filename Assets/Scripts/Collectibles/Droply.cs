using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Droply : MonoBehaviour
{
    SphereCollider droplyCollider;
    [SerializeField] GameObject droplyParticles;
    float initY;

    private void Start()
    {
        droplyCollider = GetComponent<SphereCollider>();
        droplyCollider.isTrigger = true;
        initY = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, initY + Mathf.Sin(Time.time * 5) * 0.25f + 0.25f, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Brello_Collectables collectables))
        {
            //collectables.AddDroply();
            Instantiate(droplyParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);

            AkSoundEngine.PostEvent("Dropply", WwiseManager.instance.gameObject);
        }
    }
}
