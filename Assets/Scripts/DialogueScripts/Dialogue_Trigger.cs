using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class Dialogue_Trigger : MonoBehaviour
{
    private Dialogue_Manager ui;
    private Dialogue_NPC currentVillager;

    //private MovementInput movement;
    public CinemachineTargetGroup targetGroup;

    [Space]
    [Header("Post Processing")]
    public Volume dialogueDof;

    //Input player
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.CharacterControls.Interactuable.started += OnInteractuable;
    }

    private void Start()
    {
        ui = Dialogue_Manager.instance;

        //movement = GetComponent<MovementInput>();
    }

    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
        DialogueTrigger();
    }

    private void DialogueTrigger()
    {
        if (!ui.inDialogue && currentVillager != null)
        {
            targetGroup.m_Targets[1].target = currentVillager.transform;
            //movement.active = false;
            ui.SetCharNameAndColor();
            ui.inDialogue = true;
            ui.CameraChange(true);
            ui.ClearText();
            ui.FadeUI(true, .2f, .65f);
            currentVillager.TurnToPlayer(transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentVillager = other.GetComponent<Dialogue_NPC>();
            ui.currentVillager = currentVillager;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentVillager = null;
            ui.currentVillager = currentVillager;
        }
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