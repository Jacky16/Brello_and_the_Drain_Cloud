using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class PyraPuzzleManager : MonoBehaviour
{
    public static PyraPuzzleManager instance;
    private List<PyraPuzzle> puzzleList;
    private PyraPuzzle currentPuzzle;
    private PlayerInput playerInput;
    [SerializeField] private Image fadeOut;

    InputManager inputManager;

    PlayerController player;

    [SerializeField] private CinemachineFreeLook playerCam;
    [SerializeField] private CinemachineFreeLook pyraCam;

    bool hasStartedPuzzle;
    bool canPressAgainInteractable = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = new PlayerInput();
        playerInput.CharacterControls.Interactuable.started += OnInteractuable;

        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }
    void Start()
    {
        hasStartedPuzzle = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        puzzleList = new List<PyraPuzzle>();

        PyraPuzzle[] puzzles = FindObjectsOfType<PyraPuzzle>();

        foreach(PyraPuzzle puzzle in puzzles)
        {
            puzzleList.Add(puzzle);
        }
    }

    private void OnInteractuable(InputAction.CallbackContext ctx)
    {
        if (currentPuzzle && !hasStartedPuzzle && canPressAgainInteractable)
        {
            EnablePuzzle();
        }
        else if(hasStartedPuzzle && currentPuzzle.GetAttachedPyra().GetIsInExit() && canPressAgainInteractable)
        {
            DisablePuzzle();
        }
    }


    private void EnablePuzzle()
    {
        hasStartedPuzzle = true;
        canPressAgainInteractable = false;

        fadeOut.DOFade(1f, 0.5f).OnComplete(() =>
        {
            player.BlockMovement();

            currentPuzzle.GetAttachedPyra().gameObject.SetActive(true);

            playerCam.gameObject.SetActive(false);
            pyraCam.gameObject.SetActive(true);

            pyraCam.Follow = currentPuzzle.GetAttachedPyra().transform;
            pyraCam.LookAt = currentPuzzle.GetAttachedPyra().transform;

            inputManager.SetCurrentPyra(currentPuzzle.GetAttachedPyra().GetComponent<PyraController>());

            fadeOut.DOFade(0f, 0.5f).OnComplete(()=>
            {
                canPressAgainInteractable = true;
            });
        });
    }

    private void DisablePuzzle()
    {
        hasStartedPuzzle = false;
        canPressAgainInteractable = false;

        fadeOut.DOFade(1f, 0.5f).OnComplete(() =>
        {
            currentPuzzle.GetAttachedPyra().ResetPosAndRotation();
            currentPuzzle.GetAttachedPyra().gameObject.SetActive(false);

            playerCam.gameObject.SetActive(true);
            pyraCam.gameObject.SetActive(false);

            pyraCam.Follow = currentPuzzle.GetAttachedPyra().transform;
            pyraCam.LookAt = currentPuzzle.GetAttachedPyra().transform;

            inputManager.SetCurrentPyra(null);

            fadeOut.DOFade(0f, 0.5f).OnComplete(() =>
            {
                canPressAgainInteractable = true;
                player.EnableMovement();
            });
        });
    }

    public void SetCurrentPuzzle(PyraPuzzle puzzle)
    {
        currentPuzzle = puzzle;
    }

    public PyraPuzzle GetCurrentPuzzle()
    {
        return currentPuzzle;
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
