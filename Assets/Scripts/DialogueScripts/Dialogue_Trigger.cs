using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public class Dialogue_Trigger : MonoBehaviour
{

    private Dialogue_Manager ui;
    private Dialogue_NPC currentVillager;
    //private MovementInput movement;
    public CinemachineTargetGroup targetGroup;

    [Space]
    [Header("Post Processing")]
    public Volume dialogueDof;

    void Start()
    {
        ui = Dialogue_Manager.instance;
        //movement = GetComponent<MovementInput>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !ui.inDialogue && currentVillager != null)
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
        if (other.CompareTag("Villager"))
        {
            currentVillager = other.GetComponent<Dialogue_NPC>();
            ui.currentVillager = currentVillager;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Villager"))
        {
            currentVillager = null;
            ui.currentVillager = currentVillager;
        }
    }
}
