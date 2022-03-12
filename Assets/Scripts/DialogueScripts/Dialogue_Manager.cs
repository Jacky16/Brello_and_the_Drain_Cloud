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

    [Space]
    public Volume dialogueDof;

    //Input Player
    private PlayerInput playerInput;

    private void Awake()
    {
        instance = this;
        playerInput = new PlayerInput();
    }

    private void Start()
    {
        animatedText.onDialogueFinish.AddListener(() => FinishDialogue());

        playerInput.CharacterControls.Interactuable.started += OnInteractuable;
    }

    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
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
                Sequence s = DOTween.Sequence();
                s.AppendInterval(.8f);
                s.AppendCallback(() => ResetState());
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
                    nextDialogue = false;
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
            s.Join(canvasGroup.transform.DOScale(0, time * 2).From().SetEase(Ease.OutBack));
            s.AppendCallback(() => animatedText.ReadText(currentVillager.dialogue.conversationBlock[0]));
        }
    }

    public void SetCharNameAndColor()
    {
        nameTMP.text = currentVillager.data.NPCName;
        nameTMP.color = currentVillager.data.NPCNameColor;
        nameBubble.color = currentVillager.data.NPCColor;
    }

    public void CameraChange(bool dialogue)
    {
        gameCam.SetActive(!dialogue);
        dialogueCam.SetActive(dialogue);

        //Depth of field modifier
        float dofWeight = dialogueCam.activeSelf ? 1 : 0;
        DOVirtual.Float(dialogueDof.weight, dofWeight, .8f, DialogueDOF);
    }

    public void DialogueDOF(float x)
    {
        dialogueDof.weight = x;
    }

    public void ClearText()
    {
        animatedText.text = string.Empty;
    }

    public void ResetState()
    {
        currentVillager.Reset();
        //FindObjectOfType<MovementInput>().active = true;
        inDialogue = false;
        canExit = false;
    }

    public void FinishDialogue()
    {
        if (dialogueIndex < currentVillager.dialogue.conversationBlock.Count - 1)
        {
            dialogueIndex++;
            nextDialogue = true;
        }
        else
        {
            nextDialogue = false;
            canExit = true;
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