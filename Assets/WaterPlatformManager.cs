using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WaterPlatformManager : MonoBehaviour
{
    private enum Platform_Sprite
    { ALL_ABOARD, STAY_HERE, NONE };

    private PlayerInput playerInput;
    private PlayerController player;
    private PyraAI pyra;

    [SerializeField] private Image platformMessage;
    [SerializeField] private Sprite[] platformImages;

    private CoastPoints currentCoast;
    private Vector3 closestGroundPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        playerInput = new PlayerInput();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraAI>();
    }

    private void Start()
    {
        playerInput.CharacterControls.Interactuable.started += OnInteractuable;
    }

    private void Update()
    {
        if (player.IsSwimming() && !pyra.isInPlatform && currentCoast)
        {
            //platformMessage.sprite = platformImages[(int)Platform_Sprite.ALL_ABOARD];
        }
        else if (player.IsSwimming() && pyra.isInPlatform && currentCoast)
        {
            //platformMessage.sprite = platformImages[(int)Platform_Sprite.STAY_HERE];
        }
        else
        {
            //platformMessage.sprite = null;
        }
    }

    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
        PlatformManager();
    }

    //Subirse y bajarse de la plataforma
    private void PlatformManager()
    {
        if (player.IsSwimming() && !pyra.isInPlatform && !pyra.isJumping && currentCoast)
        {
            pyra.moveToPlatform = true;
            pyra.isMovingToInteractuable = false;
            pyra.canChasePlayer = false;
        }
        else if (player.IsSwimming() && pyra.isInPlatform && !pyra.isJumping && currentCoast)
        {
            CheckForClosestPoint();
            pyra.JumpToGround(closestGroundPoint);
        }
    }

    private void CheckForClosestPoint()
    {
        float closestPoint = Mathf.Infinity;
        foreach (Transform point in currentCoast.coastPoints)
        {
            if (Vector3.Distance(point.position, player.transform.position) < closestPoint)
            {
                closestGroundPoint = point.position;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out CoastPoints closestCoast))
        {
            currentCoast = closestCoast;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentCoast = null;
        closestGroundPoint = Vector3.zero;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}