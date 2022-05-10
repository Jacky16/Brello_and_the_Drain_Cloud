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
        }

        rb.DOJump(playerAdvancedPosition, jumpForce, 1, 0.9f).OnComplete(() =>
        {
            Collider[] playerCol = Physics.OverlapSphere(new Vector3(transform.position.x, player.position.y, transform.position.z), areaDamage, playerMask);

            if (playerCol.Length > 0)
            {
                playerCol[0].GetComponent<BrelloHealth>().DoDamage(damage);
            }

            Destroy(instantiatedDecal);
            Destroy(gameObject);

            //Instanciar particulas de barro?
        });
    }

    private void Update()
    {
        //Para que siempre mire en la direccion hacia la que est� yendo.
        transform.forward = rb.velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.TryGetComponent(out BrelloHealth playerHealth))
        {
            playerHealth.DoDamage(damage);
            Destroy(instantiatedDecal);
            Destroy(gameObject);

            //Instanciar particulas de barro?
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, areaDamage);
    }

}
