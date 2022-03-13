using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WaterPlatformManager : MonoBehaviour
{
    public static WaterPlatformManager singletone;

    private enum Platform_Sprite
    { ALL_ABOARD, STAY_HERE, NONE };

    private PlayerInput playerInput;
    private PlayerController player;
    private PyraAI pyra;

    [SerializeField] private Image platformMessage;
    [SerializeField] private Sprite[] platformImages;
    private List<Transform> positionsClose;

    private CoastPoints currentCoast;
    private Vector3 closestGroundPoint;

    private struct CoastPoint
    {
        public Vector3 position;
        public float distance;
    }

    private List<CoastPoint> sortedCoastPoints = new List<CoastPoint>();

    // Start is called before the first frame update
    private void Awake()
    {
        if (singletone == null)
        {
            singletone = this;
        }
        playerInput = new PlayerInput();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pyra = GameObject.FindGameObjectWithTag("Pyra").GetComponent<PyraAI>();
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
        sortedCoastPoints.Clear();
        foreach (Transform point in currentCoast.coastPoints)
        {
            CoastPoint coastPoint;
            coastPoint.position = point.position;
            coastPoint.distance = Vector3.Distance(player.transform.position, point.position);
            sortedCoastPoints.Add(coastPoint);
        }
        sortedCoastPoints.Sort(SortbyDistance);
        closestGroundPoint = sortedCoastPoints[0].position;
    }

    public void SetCurrentCoast(CoastPoints _coast)
    {
        currentCoast = _coast;
    }

    private int SortbyDistance(CoastPoint p1, CoastPoint p2)
    {
        return p1.distance.CompareTo(p2.distance);
    }

    private void OnDrawGizmos()
    {
        if (closestGroundPoint != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(player.transform.position, closestGroundPoint);
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