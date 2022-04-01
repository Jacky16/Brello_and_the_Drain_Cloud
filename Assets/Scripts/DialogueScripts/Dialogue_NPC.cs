using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class Dialogue_NPC : MonoBehaviour
{
    public NPCData data;
    public DialogueData dialogue;

    public bool villagerIsTalking;

    private TMP_Dialogue animatedText;
    private Dialogue_AudioController dialogueAudio;
    private Animator animator;
    public Renderer eyesRenderer;

    public Transform particlesParent;

    void Start()
    {
        dialogueAudio = GetComponent<Dialogue_AudioController>();
        //animator = GetComponent<Animator>();
        animatedText = Dialogue_Manager.instance.animatedText;
        animatedText.onEmotionChange.AddListener((newEmotion) => EmotionChanger(newEmotion));
        animatedText.onAction.AddListener((action) => SetAction(action));
    }

    public void EmotionChanger(Emotion e)
    {
        if (this != Dialogue_Manager.instance.currentVillager)
            return;

        //animator.SetTrigger(e.ToString());

        if (e == Emotion.surprised) 
           eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0.46f, -0.2f));

        if (e == Emotion.angry) 
           eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0.46f, 0.2f));

        if (e == Emotion.sad) 
           eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0f, -0.2f));
    }

    public void SetAction(string action)
    {
        if (this != Dialogue_Manager.instance.currentVillager)
            return;

        if (action == "shake")
        {
            Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
        else
        {
            PlayParticle(action);

            if (action == "sparkle")
            {
                dialogueAudio.effectSource.clip = dialogueAudio.sparkleClip;
                dialogueAudio.effectSource.Play();
            }
            else if (action == "rain")
            {
                dialogueAudio.effectSource.clip = dialogueAudio.rainClip;
                dialogueAudio.effectSource.Play();
            }
        }
    }

    public void PlayParticle(string x)
    {
        if (particlesParent.Find(x + "Particle") == null)
            return;
        particlesParent.Find(x + "Particle").GetComponent<ParticleSystem>().Play();
    }

    public void Reset()
    {
        //animator.SetTrigger("normal");
        //eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0f, 0.2f));
    }

    public void TurnToPlayer(Vector3 playerPos)
    {
        //transform.DOLookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z), Vector3.Distance(transform.position, playerPos) / 5);
        //string turnMotion = isRightSide(transform.forward, playerPos, Vector3.up) ? "rturn" : "lturn";
        //animator.SetTrigger(turnMotion);
    }

    //https://forum.unity.com/threads/left-right-test-function.31420/
    public bool isRightSide(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 right = Vector3.Cross(up.normalized, fwd.normalized);        // right vector
        float dir = Vector3.Dot(right, targetDir.normalized);
        return dir > 0f;
    }
}
