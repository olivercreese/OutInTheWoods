using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static Sheep;

public class MalusAranea : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject player;

    private NewInputManager inputManager;
    private bool aggro;

    //animation
    public enum monsterState { Chasing, Idle, Attacking, wandering, wait, searching}
    public monsterState currentState;
    private float restTimer;
    private float searchTimer;
    //line of sight 

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
   
        if (canSeePlayer() && currentState!=monsterState.searching) currentState = monsterState.Chasing;

        switch (currentState)
        {
            case monsterState.Chasing:
                Chasing();
                anim.SetBool("isChasing", true);
                NavAgent.SetDestination(player.transform.position);
                break;
            case monsterState.Idle:
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", false);
                Idle();
                break;
            case monsterState.Attacking:
                anim.SetBool("isChasing", false);
                Attacking();
                break;
            case monsterState.wandering:
                wandering();
                break;
            case monsterState.wait:
                wait();
                break;
            case monsterState.searching:
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", true);
                Searching();
                break;
        }
    }

    protected void Chasing()
    {
        NavAgent.acceleration = 15;
        anim.speed = 1;
        NavAgent.speed = 20;
        NavAgent.autoBraking = false;
        aggro = true;

        if (NavAgent.remainingDistance <= 1)
        {
            currentState = monsterState.Attacking;
        }

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
        anim.speed = 1;
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
    protected void Attacking()
    {
        anim.SetTrigger("Attack");
        NavAgent.speed = 0;

        
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
        if (searchTimer > 3) aggro = false;
        if (searchTimer >= 10)
        {
            searchTimer = 0;
            currentState = monsterState.Idle;
        }
        //DetectAngle = 180;
        //DetectRange = 40;

        if (canSeePlayer())
        {
            currentState = monsterState.Chasing;
        }

        if (NavAgent.destination == null || NavAgent.remainingDistance < 1) NavAgent.SetDestination(RandomNavmeshLocation(5));

        NavAgent.speed = 2;
        NavAgent.acceleration = 5;
        NavAgent.autoBraking = true;
        anim.SetBool("isWandering", true);

    }


    protected bool canSeePlayer()
    {
        isInAngle = false;
        isInRange = false;
        isNotHidden = false;
        
        if (inputManager.Crouch && !aggro)
        {
            DetectRange = 15;
        }
        else
        {
            DetectRange = 100;
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

    } //https://www.youtube.com/watch?v=kMHwy-unZ5M
}
