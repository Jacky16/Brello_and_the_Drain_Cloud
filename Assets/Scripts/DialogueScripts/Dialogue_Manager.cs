using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using Cinemachine;
using UnityEngine.InputSystem;

public class Dialogue_Manager : MonoBehaviour
{
    public bool inDialogue;

    public static Dialogue_Manager instance;

    public CanvasGroup canvasGroup;
    public TMP_Dialogue animatedText;
    public Image nameBubble;
    public TextMeshProUGUI nameTMP;

    //[HideInInspector]
    public Dialogue_NPC currentVillager;

    private int dialogueIndex;
    public bool canExit;
    public bool nextDialogue;

    [Space]
    [Header("Cameras")]
    public GameObject gameCam;

    public GameObject dialogueCam;

    //Input Player
    private PlayerInput playerInput;
    private PlayerController player;
    private Dialogue_Trigger dialogueTrigger;

    public bool canStartTalking;

    private void Awake()
    {
        instance = this;
        playerInput = new PlayerInput();
    }

    private void Start()
    {
        canStartTalking = false;

        dialogueTrigger = GameObject.FindGameObjectWithTag("Player").GetComponent<Dialogue_Trigger>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        animatedText.onDialogueFinish.AddListener(() => FinishDialogue());

        playerInput.CharacterControls.Interactuable.started += OnInteractuable;
    }

    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
        if(canStartTalking)
            DialogueManager();
    }
    private void DialogueManager()
    {
        if (inDialogue)
        {
            if (canExit)
            {               
                CameraChange(false);
                FadeUI(false, .2f, 0);

                transform.DOScale(1f, 0.6f).OnComplete(() =>
                 {
                     dialogueTrigger.canStartAgain = true;
                     canStartTalking = false;
                     ResetState();
                 });
            }
            else
            {
                if (!nextDialogue)
                {
                    animatedText.DisplayCurrentDialogue();
                    FinishDialogue();
                }
                else
                {
                    animatedText.ReadText(currentVillager.dialogue.conversationBlock[dialogueIndex]);
                }
            }
        }
    }

    public void FadeUI(bool show, float time, float delay)
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(delay);
        s.Append(canvasGroup.DOFade(show ? 1 : 0, time));

        if (show)
        {
            dialogueIndex = 0;
            s.Join(canvasGroup.transform.DOScale(0f, time * 2).From().SetEase(Ease.OutBack));
            s.AppendCallback(() => animatedText.ReadText(currentVillager.dialogue.conversationBlock[dialogueIndex]));
            s.OnComplete(() => canStartTalking = true);
        }
    }

    public void SetCharNameAndColor()
    {
        nameTMP.text = currentVillager.data.NPCName;
        //nameTMP.color = currentVillager.data.NPCNameColor;
        //nameBubble.color = currentVillager.data.NPCColor;
    }

    public void CameraChange(bool dialogue)
    {
        gameCam.SetActive(!dialogue);
        dialogueCam.SetActive(dialogue);
    }

    public void ClearText()
    {
        animatedText.text = string.Empty;
    }

    public void ResetState()
    {
        currentVillager.Reset();
        player.EnableMovement();
        inDialogue = false;
        canExit = false;
    }

    public void FinishDialogue()
    {
        if(currentVillager.dialogue.conversationBlock == null)
        {
            Debug.Log("ConversationBlock es nulo");
        }
        else if(currentVillager.dialogue == null)
        {
            Debug.Log("Dialogue es nulo");
        }
        else if(currentVillager == null)
        {
            Debug.Log("Villager es nulo");
        }

        if (currentVillager)
        {
            if (dialogueIndex < currentVillager.dialogue.conversationBlock.Count - 1)
            {
                dialogueIndex++;
                nextDialogue = true;
                canExit = false;
            }

            else
            {
                nextDialogue = false;
                canExit = true;
            }
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