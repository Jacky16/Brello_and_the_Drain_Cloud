using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyraController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    private Vector2 axis;
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 camDir;

    private Vector3 initPos;
    private Vector3 initRot;

    private bool isMovementPressed;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 20;
    [SerializeField] private float rotationSpeed = 15f;

    private bool canMove = true;

    [Header("Ground Checker settings")]
    [SerializeField] Transform posCheckerGround;
    [SerializeField] float radiusCheck = .25f;
    [SerializeField] LayerMask groundLayerMask;
    bool isGrounded;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10;

    bool isInExit = false;
    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        initPos = transform.position;
        initRot = transform.eulerAngles;
    }
    private void Update()
    {
        HandleRotation();
        HandleAnimation();
        
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void HandleAnimation()
    {
        animator.SetFloat("Speed", speed);
        animator.SetBool("isWalking", isMovementPressed);
    }

    private void HandleRotation()
    {
        if (canMove)
        {
            Vector3 positionToLookAt = (axis.x * camRight + axis.y * camForward);

            //Setear la rotacion en la cual va a girar

            positionToLookAt.y = 0.0f;

            //Obtenemos la rotacion actual del Player
            Quaternion currentRotation = transform.rotation;

            if (isMovementPressed)
            {
                //Hacemos una interpolaci�n en la rotacion que va a girar cuando el player se mueve
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                rb.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    private void Movement()
    {
        if (canMove)
        {
            Vector3 dir = CamDirection() * speed;
            dir.y = rb.velocity.y;
            rb.AddForce(speed * CamDirection(),ForceMode.Acceleration);
        }
    }
    Vector3 CamDirection()
    {
        camForward = Camera.main.transform.forward.normalized;
        camRight = Camera.main.transform.right.normalized;
        camDir = (axis.x * camRight + axis.y * camForward);
        camDir.y = 0;
        return camDir;
    }

    #region Inputs setters

    public void SetAxis(Vector2 _value)
    {
        axis = _value;
    }

    public void SetMovementPressed(bool _value)
    {
        isMovementPressed = _value;
    }

    #endregion Inputs setters

    public void SetIsInExit(bool _value)
    {
        isInExit = _value;
    }

    public bool GetIsInExit()
    {
        return isInExit;
    }

    public void ResetPosAndRotation()
    {
        transform.position = initPos;
        transform.eulerAngles = initRot;
    }
}
