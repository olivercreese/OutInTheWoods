using System.Net;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Sheep;

public class MalusAranea : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject player;

    private NewInputManager inputManager;
    private bool canSeeWhenCrouched;

    //animation
    public enum monsterState { Chasing, Idle, wandering, wait, searching}
    public monsterState currentState;
    private float restTimer;
    private float searchTimer;
    //line of sight 
    private bool aggro;
    public float DetectionTime = 3f;
    private float detectionTimer;
    public float DetectRange = 10f;
    public float DetectAngle = 45f;
    bool isInAngle, isInRange, isNotHidden;

    protected void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = monsterState.Idle;
        player = GameObject.FindWithTag("Player");
        inputManager = player.GetComponent<NewInputManager>();
    }

    protected void Update()
    {

        if (canSeePlayer() && currentState != monsterState.searching && aggro) { 
             currentState = monsterState.Chasing;
        }

        detectionMeter();
        switch (currentState)
        {
            case monsterState.Chasing:
                NavAgent.acceleration = 50;
                anim.speed = 1.5f;
                NavAgent.autoBraking = false;
                NavAgent.angularSpeed = 60;
                canSeeWhenCrouched = true;
                Chasing();
                anim.SetBool("isChasing", true);
                anim.SetBool("isWandering", false);
                NavAgent.SetDestination(player.transform.position);
                break;
            case monsterState.Idle:
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", false);
                Idle();
                anim.speed = 1;
                break;
            case monsterState.wandering:
                wandering();
                break;
            case monsterState.wait:
                wait();
                break;
            case monsterState.searching:
                NavAgent.speed = 2;
                NavAgent.acceleration = 5;
                NavAgent.autoBraking = true;
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", true);
                canSeeWhenCrouched = false;
                aggro = false;
                Searching();
                break;
        }
    }

    protected void detectionMeter()
    {
        if (isInAngle && isInRange && isNotHidden && !aggro)
        {
            currentState = monsterState.wait;
            
            detectionTimer += Time.deltaTime;
            transform.LookAt(player.transform.position);
            if (detectionTimer >= DetectionTime)
            {
                detectionTimer = 0;
                aggro = true;
            }
        }
        else if (!isInAngle && !isInRange && !isNotHidden && !aggro)
            detectionTimer -= Time.deltaTime / 2;
    }

    protected void Chasing()
    {
        if (NavAgent.remainingDistance <= 2)
        {
            anim.SetTrigger("Attack");
            NavAgent.speed = 0;
        }
        else NavAgent.speed = 20;


        if (!canSeePlayer())
        {
            currentState = monsterState.searching;
        }
    }

    protected void wait()
    {
        restTimer += Time.deltaTime;
        if (restTimer >= Random.Range(3, 10))
        {
            restTimer = 0;
            currentState = monsterState.Idle;
        }
    }

    protected void Idle()
    {
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                currentState = monsterState.wandering;
                anim.SetBool("isWandering", true);
                NavAgent.SetDestination(RandomNavmeshLocation(40));
                break;
            case 1:
                currentState = monsterState.wait;
                break;
        }
    }


    protected void wandering()
    {
        NavAgent.acceleration = 8;
        NavAgent.speed = 10;
        NavAgent.autoBraking = true;
        if (NavAgent.remainingDistance <= 0.5f)
        {
            currentState = monsterState.Idle;
            anim.SetBool("isWandering", false);
        }
    }

    protected void Searching()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer >= 10)
        {
            searchTimer = 0;
            currentState = monsterState.Idle;
        }


        if (canSeePlayer())
        {
            currentState = monsterState.Chasing;
        }

        if (NavAgent.destination == null || NavAgent.remainingDistance < 1) NavAgent.SetDestination(RandomNavmeshLocation(5));
    }


    protected bool canSeePlayer()
    {
        isInAngle = false;
        isInRange = false;
        isNotHidden = false;
        
        if (inputManager.Crouch && !canSeeWhenCrouched)
        {
            DetectRange = 10;
        }
        else
        {
            DetectRange = 60;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < DetectRange) isInRange = true;

        RaycastHit hit;        
        if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 6,transform.position.z), (player.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == player) isNotHidden = true;
        }

        Vector3 side1 = player.transform.position - transform.position;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);
        if (angle < DetectAngle && angle > -1 * DetectAngle) isInAngle = true;

        if (isInAngle && isInRange && isNotHidden) return true;
        else return false;

    } 



}
