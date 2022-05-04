using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;

public class DialogueTutorial : MonoBehaviour
{
    //Variables del canvas del dialogo
    CanvasGroup dialogueGroup;
    TextMeshProUGUI dialogueText;
    TextMeshProUGUI dialogueName;
    Image dialogueNameBackColor;

    [Header("Variables del dialogo")]
    [SerializeField] DialogueObjectData dialogueObject;
    private string[] dialogueInParts;
    private string displayText;

    [SerializeField] float speed = 0.05f;
    private int index;

    //Booleanos para controlar cuando se puede pulsar la E
    private bool inDialogue;
    private bool loadingDialogue;
    private bool reloadDialogue;
    private bool reloadingDialogue;
    [SerializeField] bool unlocksGlide;
    [SerializeField] bool unlocksAttack;
    [SerializeField] bool isCinematicDialogue;
    [SerializeField] GameObject particles;

    //Variables del player
    PlayerController player;
    private PlayerInput playerInput;

    //Cámaras
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject dialogueCam;
    [SerializeField] Animator character;

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.canGlide = false;
        player.canAttack = false;
        dialogueGroup = GameObject.FindGameObjectWithTag("DialogueGroup").GetComponent<CanvasGroup>();
        if (!isCinematicDialogue) { 
            dialogueText = dialogueGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
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
        if (player && !isCinematicDialogue)
        {
            if (dialogueText.maxVisibleCharacters == displayText.Length && !reloadingDialogue && inDialogue && !loadingDialogue)
            {
                NextDialogue();
            }
            else if (!loadingDialogue && !reloadingDialogue && inDialogue)
            {
                StopAllCoroutines();

                for (int i = 0; i < dialogueInParts.Length; i++)
                {
                    dialogueText.maxVisibleCharacters = displayText.Length;
                }
            }
        }
    }

    #region CoreFunctions
    void StartDialogue()
    {
        index = 0;

        ReadCurrentDialogue();

        StartCoroutine(TypeDialogue());
    }
    void ReadCurrentDialogue()
    {
        //IMPORTANTE PARA ENTENDER ESTE CODIGO:
        //Al separar por tags, lo de fuera de un tag siempre estará en una posicion par en el array
        //y lo de dentro de un tag siempre estará en una posicion impar.
        //Es por esta razon por la que se verá como compruebo si el número es par o impar en varias ocasiones.

        //Ponemos a 0 el numero de caracteres visibles para cargar el texto en 2o plano.
        dialogueText.maxVisibleCharacters = 0;

        //Dividimos el texto actual separandolo cuando encuentre un tag
        dialogueInParts = dialogueObject.dialogue.conversationBlock[index].Split('<', '>');

        displayText = string.Empty;
        for (int i = 0; i < dialogueInParts.Length; i++)
        {
            //Añadimos el texto tal cual
            if (i % 2 == 0)
                displayText += dialogueInParts[i];
            //Si no es un tag hecho por nosotros, lo ponemos para que TMP haga lo suyo.
            else if (!isCustomTag(dialogueInParts[i].Replace(" ", "")))
                displayText += $"<{dialogueInParts[i]}>";
        }

        dialogueText.text = displayText;
    }
    IEnumerator TypeDialogue()
    {
        int subCounter = 0;
        int visibleCounter = 0;

        while (subCounter < dialogueInParts.Length)
        {
            if (subCounter % 2 == 1)
            {
                yield return EvaluateTag(dialogueInParts[subCounter].Replace(" ", ""));
            }
            else
            {
                while (visibleCounter < dialogueInParts[subCounter].Length)
                {
                    visibleCounter++;
                    dialogueText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(speed);
                }
                visibleCounter = 0;
            }
            subCounter++;
        }
        yield return null;

        WaitForSeconds EvaluateTag(string tag)
        {
            if (tag.Length > 0)
            {
                if (tag.StartsWith("speed="))
                {
                    speed = float.Parse(tag.Split('=')[1], CultureInfo.InvariantCulture);
                }
                else if (tag.StartsWith("pause="))
                {
                    return new WaitForSeconds(float.Parse(tag.Split('=')[1], CultureInfo.InvariantCulture));
                }
                else if (tag.StartsWith("emotion="))
                {
                    EmotionHandler((Emotion)System.Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
                }
                else if (tag.StartsWith("action="))
                {
                    ActionHandler(tag.Split('=')[1]);
                }
                else if (tag.StartsWith("shop"))
                {
                    ShopHandler();
                }
                else if (tag.StartsWith("end"))
                {
                    //CameraHandler(true);
                    DialogueCanvasHandler(false);
                }
                else if (tag.StartsWith("continue"))
                {
                    NextDialogue();
                }
            }
            return null;
        }
    }
    void NextDialogue()
    {
        if (index < dialogueObject.dialogue.conversationBlock.Count - 1)
        {
            index++;

            ReadCurrentDialogue();
            StartCoroutine(TypeDialogue());
        }
        else
        {
            //CameraHandler(true);
            DialogueCanvasHandler(false);
        }
    }
    IEnumerator ReloadDialogue()
    {
        yield return new WaitForSeconds(1.25f);
        inDialogue = false;
        reloadingDialogue = false;
        speed = 0.05f;
    }
    #endregion

    #region DialogueAnalyzers

    bool isCustomTag(string tag)
    {
        return tag.StartsWith("speed=")
            || tag.StartsWith("pause=")
            || tag.StartsWith("emotion=")
            || tag.StartsWith("action=")
            || tag.StartsWith("shop")
            || tag.StartsWith("end")
            || tag.StartsWith("continue");
    }

    #endregion

    #region DialogueHandlers

    private void CameraHandler(bool playerCamera)
    {
        playerCam.SetActive(playerCamera);
        dialogueCam.SetActive(!playerCamera);
    }

    private void DialogueCanvasHandler(bool showCanvas)
    {
        if (showCanvas)
        {
            if (character)
            {
                character.SetTrigger("Appear");
                Instantiate(particles, new Vector3(character.transform.position.x, character.transform.position.y + 2f, character.transform.position.z), Quaternion.identity);
                character.transform.DOLookAt(player.transform.position, 0.5f, AxisConstraint.Y);
                player.transform.DOLookAt(character.transform.position, 0.5f, AxisConstraint.Y);
            }

            dialogueText.text = string.Empty;
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

            if (character)
            {
                character.SetTrigger("Disappear");
            }

            dialogueGroup.transform.DOScale(0f, 1f).OnComplete(() =>
            {
                dialogueText.text = string.Empty;
                dialogueGroup.gameObject.SetActive(false);
                player.EnableMovement();

                if (unlocksGlide)
                {
                    player.canGlide = true;
                }
                else if (unlocksAttack)
                {
                    player.canAttack = true;
                }

                Instantiate(particles, new Vector3(character.transform.position.x, character.transform.position.y + 2f, character.transform.position.z), Quaternion.identity);

                Destroy(gameObject);
            });
        }
    }

    #endregion

    #region CollisionCheckers
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            player = playerController;
            playerCam.GetComponent<CinemachineVirtualCamera>().LookAt = character.transform;
            loadingDialogue = true;
           // CameraHandler(false);
            DialogueCanvasHandler(true);
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

    #region CustomTagHandlers

    private void EmotionHandler(Emotion emotion)
    {
        Debug.Log("He recibido la emocion: " + emotion.ToString());
    }

    private void ActionHandler(string action)
    {
        Debug.Log("He recibido la acción: " + action);
    }

    private void ShopHandler()
    {
        Debug.Log("Abrir la tienda");
    }

    #endregion

    #region InputFunctions
    private void OnEnable()
    {
        playerInput.Enable();

        if (isCinematicDialogue)
        {
            dialogueText = GameObject.FindGameObjectWithTag("CinematicText").GetComponent<TextMeshProUGUI>();
            inDialogue = true;
            StartDialogue();
        }
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    #endregion

}
