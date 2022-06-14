using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CoalBall : MonoBehaviour
{
    Transform player;
    Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject attachedDecal;
    [SerializeField] int damage;
    GameObject instantiatedDecal;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float areaDamage;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        Vector3 playerAdvancedPosition = player.position;

        Physics.Raycast(playerAdvancedPosition, Vector3.down, out RaycastHit raycastHit, whatIsGround);

        if (raycastHit.collider)
        {
            instantiatedDecal = Instantiate(attachedDecal, raycastHit.point, Quaternion.identity);
            instantiatedDecal.transform.forward = raycastHit.normal;
            //playerAdvancedPosition = new Vector3(playerAdvancedPosition.x, raycastHit.point.y, playerAdvancedPosition.z);
        }

        rb.DOJump(playerAdvancedPosition, jumpForce, 1, 0.9f).OnComplete(() =>
        {
            Collider[] playerCol = Physics.OverlapSphere(new Vector3(transform.position.x, player.position.y, transform.position.z), areaDamage, playerMask);

            if (playerCol.Length > 0)
            {
                playerCol[0].GetComponent<BrelloHealth>().DoDamage(damage);
            }

            Instantiate(explosionParticles, transform.position, Quaternion.identity);

            Destroy(instantiatedDecal);
            Destroy(gameObject);           
        });
    }

    private void Update()
    {
        //Para que siempre mire en la direccion hacia la que está yendo.
        transform.forward = rb.velocity;

        transform.localEulerAngles = Vector3.one * Mathf.Sin(Time.time) * 360f;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.TryGetComponent(out BrelloHealth playerHealth))
        {
            playerHealth.DoDamage(damage);

            Instantiate(explosionParticles, transform.position, Quaternion.identity);

            Destroy(instantiatedDecal);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, areaDamage);
    }

}
