using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlatformMovement : MonoBehaviour
{
    Vector3 initPos;
    enum MovementType { UPWARDS, DOWNWARDS, NONE };
    [SerializeField] MovementType movementType;
    float randomMovementSpeed;
    float randomMovementOffset;
    [SerializeField] Transform posToRespawn;
    TutorialManager tutorialManager;
    Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        tutorialManager = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialManager>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        initPos = rb.position;
        randomMovementSpeed = Random.Range(0.3f, 0.5f);
        randomMovementOffset = Random.Range(0.3f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
       if (movementType == MovementType.UPWARDS)
       {
           rb.position = initPos + new Vector3(0, Mathf.Sin(Time.time * randomMovementSpeed) * randomMovementOffset + randomMovementOffset, 0);
       }
       else if(movementType == MovementType.DOWNWARDS)
       {
           rb.position = initPos + new Vector3(0, Mathf.Sin(Time.time * -randomMovementSpeed) * randomMovementOffset + randomMovementOffset, 0);
       }
    }

    public Vector3 GetPosToRespawn()
    {
        return posToRespawn.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            tutorialManager.SetLastPlatform(this);
        }
    }
}
