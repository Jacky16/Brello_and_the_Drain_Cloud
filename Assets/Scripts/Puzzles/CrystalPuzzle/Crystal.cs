using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crystal : MonoBehaviour
{
    [SerializeField] private CrystalPuzzle manager;

    public bool isShiny;

    //Variables del player
    PlayerController player;
    private PlayerInput playerInput;

    private bool canBeInteracted = true;

    [SerializeField] private ParticleSystem particleShiny;
    [SerializeField] private ParticleSystem particleActivated;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.CharacterControls.Interactuable.started += OnInteract;

        particleShiny.Stop();
        particleActivated.Stop();

    }

    private void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            player = playerController;
            
        }

        if (other.CompareTag("Pyra") && isShiny){
            particleShiny.Play();
            Debug.Log("Shine shine");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            player = null;
        }

        if (other.CompareTag("Pyra")){
            particleShiny.Stop();
        }
    }

    public IEnumerator ResetCrystal()
    {
        canBeInteracted = false;

        particleActivated.Stop();

        yield return new WaitForSeconds(1.25f);


        canBeInteracted = true;
    }


    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (player && canBeInteracted)
        {
            if (isShiny)
            {
                particleActivated.Play();
                manager.AddGoodOne();
            }
            else
            {
                manager.ResetCrystals();
            }
        }
    }

    private bool PlayerIsLooking(PlayerController player)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        return Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.forward), 5f, LayerMask.GetMask("Interactable"));

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
