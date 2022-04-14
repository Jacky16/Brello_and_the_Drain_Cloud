using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueObject : MonoBehaviour
{
    CanvasGroup dialogueGroup;
    TextMeshProUGUI dialogueText;
    TextMeshProUGUI dialogueName;
    Image dialogueNameBackColor;
    [SerializeField] DialogueObjectData dialogueObject;
    [SerializeField] float speed;
    PlayerController player;
    private int index;

    private bool inDialogue;
    private bool loadingDialogue;
    private bool reloadDialogue;
    private bool reloadingDialogue;

    private PlayerInput playerInput;

    private void Awake()
    {
        reloadingDialogue = false;
        reloadDialogue = false;
        inDialogue = false;
        loadingDialogue = false;

        playerInput = new PlayerInput();
        playerInput.CharacterControls.Interactuable.started += OnInteractuable;
    }
    private void Start()
    {
        dialogueGroup = GameObject.FindGameObjectWithTag("DialogueGroup").GetComponent<CanvasGroup>();
        dialogueText = dialogueGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogueNameBackColor = dialogueGroup.transform.GetChild(1).GetComponent<Image>();
        dialogueName = dialogueGroup.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (reloadDialogue)
        {
            reloadDialogue = false;
            reloadingDialogue = true;
            StartCoroutine(ReloadDialogue());
        }
    }
    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
        if (player)
        {
            if (!inDialogue && !loadingDialogue)
            {
                loadingDialogue = true;
                CameraHandler(false);
                DialogueCanvasHandler(true);
            }
            else if (dialogueText.text == dialogueObject.dialogue.conversationBlock[index] && !reloadingDialogue && inDialogue && !loadingDialogue)
            {
                NextDialogue();
            }
            else if(!loadingDialogue && !reloadingDialogue && inDialogue)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueObject.dialogue.conversationBlock[index];
            }
        }
    }
        
    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeDialogue());
    }

    IEnumerator ReloadDialogue()
    {
        yield return new WaitForSeconds(1.25f);
        inDialogue = false;
        reloadingDialogue = false;
    }

    IEnumerator TypeDialogue()
    {
        foreach(char letter in dialogueObject.dialogue.conversationBlock[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }

    void NextDialogue()
    {
        if(index < dialogueObject.dialogue.conversationBlock.Count - 1)
        {
            index++;
            dialogueText.text = string.Empty;
            StartCoroutine(TypeDialogue());
        }
        else
        {
            CameraHandler(true);
            DialogueCanvasHandler(false);
        }
    }

    #region DialogueFunctions

    private void CameraHandler(bool playerCamera)
    {

    }

    private void DialogueCanvasHandler(bool showCanvas)
    {
        if (showCanvas)
        {
            player.BlockMovement();
            dialogueName.text = dialogueObject.NPCName;
            dialogueNameBackColor.color = dialogueObject.DialogueObjectColor;
            dialogueName.color = dialogueObject.DialogueObjectNameColor;

            dialogueGroup.gameObject.SetActive(true);

            dialogueGroup.DOFade(1f, 0.75f);
            dialogueGroup.transform.DOScale(1f, 0.75f).OnComplete(() => {
                loadingDialogue = false;
                inDialogue = true;
                StartDialogue();
            });
        }
        else
        {
            reloadDialogue = true;
            dialogueGroup.DOFade(0f, 0.75f);

            dialogueGroup.transform.DOScale(0f, 0.75f).OnComplete(() =>
            {
                dialogueText.text = string.Empty;
                dialogueGroup.gameObject.SetActive(false);
                player.EnableMovement();
            });
        }
    }

    #endregion
    #region CollisionCheckers
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController playerController))
        {
            player = playerController;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            player = null;
        }
    }
    #endregion

    #region InputFunctions
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    #endregion
}
