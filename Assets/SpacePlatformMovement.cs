using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePlatformMovement : MonoBehaviour
{
    Vector3 initPos;
    enum MovementType { UPWARDS, DOWNWARDS };
    [SerializeField] MovementType movementType;
    float randomMovementSpeed;
    float randomMovementOffset;
    [SerializeField] Vector3 posToRespawn;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        randomMovementSpeed = Random.Range(0.3f, 0.5f);
        randomMovementOffset = Random.Range(0.3f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (movementType == MovementType.UPWARDS)
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * randomMovementSpeed) * randomMovementOffset + randomMovementOffset, 0);
        }
        else
        {
            transform.position = initPos + new Vector3(0, Mathf.Sin(Time.time * -randomMovementSpeed) * randomMovementOffset + randomMovementOffset, 0);
        }
    }
}
