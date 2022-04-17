using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PsTom : MonoBehaviour
{
    //References/Components
    NavMeshAgent navMeshAgent;
    PsTomHealth psTomHealth;
    GameObject player;
    Collider collider;

    //Bools variables
    bool canAssaultPlayer = true;
    bool isAssaltingPlayer;
    bool isJumpingAttack;

    Vector3 startPosition;

    [Header("Settings Trash Settings")]
    [SerializeField] float throwTrashPower;
    [SerializeField] float timeToThrowTrash;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] Transform trashSpawn;
    float counterTrash = 0;
    

    [Header("Settins Assault Attack")]
    [SerializeField] float speedAssault;
    [SerializeField] float accelerationAssault;

    [Header("Stune Settings")]
    [SerializeField] float timeStuned;

    [Header("Return Jump Settings")]
    [SerializeField] float jumpReturnPower;
    [SerializeField] float jumpReturnDuration;
    

    [Header("Attack Jump Settings")]
    [SerializeField] float jumpAttackPower;
    [SerializeField] float jumpAttackDuration;
    [SerializeField] float impulseForceOnPlayer = 150;

    [Header("Boiler Settings")]
    [SerializeField] GameObject[] boilers;
    int maxBoilers;
    int currentBoilersActive = 0;


    enum Phases {PHASE_1,PHASE_2,PHASE_3,PHASE_4,PHASE_5,PHASE_6 }
    Phases currentPhase = Phases.PHASE_1;

    PsTomHealth bossHealth;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        psTomHealth = GetComponent<PsTomHealth>();
        player = GameObject.FindGameObjectWithTag("Player");
        collider = GetComponent<Collider>();
    }
    private void Start()
    {
        startPosition = transform.position;
        maxBoilers = boilers.Length;
    }
    private void Update()
    {
       
    }

    void BossManager()
    {
        switch (currentPhase)
        {
            case Phases.PHASE_1:
                break;
            case Phases.PHASE_2:
                break;
            case Phases.PHASE_3:
                break;
            case Phases.PHASE_4:
                break;
            case Phases.PHASE_5:
                break;
            case Phases.PHASE_6:
                break;
        }
    }

    #region Main Attacks
    void ShootPunch()
    {

    }
    void Trash()
    {
        counterTrash += Time.deltaTime;
        if(counterTrash >= timeToThrowTrash)
        {
            counterTrash = 0;
            GameObject go1 = Instantiate(trashPrefab, trashSpawn.position, Quaternion.identity);

            Vector3 playerDir_1 = (player.transform.position - go1.transform.position).normalized;
            
            go1.GetComponent<Rigidbody>().AddForce(playerDir_1 * throwTrashPower, ForceMode.Impulse);

        }
    }
    void InvokeBoiler()
    {
        if (currentBoilersActive < maxBoilers)
        {
            boilers[currentBoilersActive].SetActive(true);
            currentBoilersActive++;
        }
    }
    void PunchNormal()
    {

    }
    void JumpAttack()
    {
        Vector3 currentPosPlayer = player.transform.position;
        if (!isJumpingAttack)
        {
            isJumpingAttack = true;

            navMeshAgent.enabled = false;

            Sequence sequence = DOTween.Sequence();
            
            sequence.AppendInterval(2);

            //Start
            sequence.AppendCallback(() => collider.isTrigger = true);

            //Jump Attack
            sequence.Append(transform.DOJump(currentPosPlayer + new Vector3(0.5f,.5f,0.5f), jumpAttackPower, 1, jumpAttackDuration));
                
            //On complete
            sequence.AppendCallback(() => AddImpulseToPlayer());

            sequence.AppendInterval(.5f);

            sequence.AppendCallback(() => collider.isTrigger = false);

            sequence.AppendInterval(timeStuned);

            //Jump return
            sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));
            sequence.OnComplete(() => isJumpingAttack = false);
           
        }
    }
    void Assault()
    {
        Vector3 currentPosPlayer = player.transform.position;
        if (!isAssaltingPlayer && canAssaultPlayer)
        {
            canAssaultPlayer = false;
            isAssaltingPlayer = true;

            navMeshAgent.SetDestination(currentPosPlayer);

            navMeshAgent.speed = speedAssault;
            navMeshAgent.acceleration = accelerationAssault;
        }

        float distance = Vector3.Distance(transform.position, navMeshAgent.destination);
        if(distance <= navMeshAgent.stoppingDistance && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }
    
    #endregion


    Sequence JumpReturn()
    {
        navMeshAgent.enabled = false;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(timeStuned);

        sequence.Append(transform.DOJump(startPosition, jumpReturnPower, 1, jumpReturnDuration));

        sequence.OnStart(() =>
        { 
            navMeshAgent.enabled = false;
        });
        sequence.OnComplete(() =>
        {
            navMeshAgent.enabled = true;

            isAssaltingPlayer = false;
            canAssaultPlayer = true;
        
        });
        return sequence;
    }
    public void ChangePhase(float _lifeBoss)
    {
        if (_lifeBoss > 66)
        {
            currentPhase = Phases.PHASE_1;
        }
        if (_lifeBoss <= 66)
        {
            currentPhase = Phases.PHASE_2;
        }
        else if (_lifeBoss <= 33)
        {
            currentPhase = Phases.PHASE_3;
        }

    }
    void AddImpulseToPlayer()
    {
        if (CheckIfPlayerInside())
        {
            Vector3 dir = player.transform.position - transform.position;
            player.GetComponent<AddForceCharacterController>().AddImpact(dir, 150);
                
            //player.GetComponent<PlayerController>().HandleAddForce(dir, 20);
        }
    }
    bool CheckIfPlayerInside()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out PlayerController _pc)){
                return true;
            }
        }
        return false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
        if (collision.collider.CompareTag("Wall") && isAssaltingPlayer)
        {
            isAssaltingPlayer = false;
            JumpReturn();
        }
    }
    


}
