using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class InmundaFormica : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject player;
    //[SerializeField] Rigidbody rb;
    //[SerializeField] float jumpForce = 1;
    //[SerializeField] private LayerMask groundLayerCheck;
    private AudioManager audioManager;
    [SerializeField] AudioClip howl;
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip Alert;
    [SerializeField] AudioSource MainAudioSrc;

    private NewInputManager inputManager;
    private bool canSeeWhenCrouched;
    //private bool grounded;

    //animation
    public enum monsterState { Chasing, Idle, wandering, wait, searching, alerted }
    public monsterState currentState;
    private float restTimer;
    private float searchTimer;
   // public Vector3 leapTarget;
    //line of sight 
    private bool aggro; 
    public float DetectionTime = 3f;
    private float detectionTimer;
    private float howlTimer;
    public float DetectRange = 10f;
    public float DetectAngle = 45f;
    //private bool canLeap;
    bool isInAngle, isInRange, isNotHidden;

    protected void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = monsterState.Idle;
        player = GameObject.FindWithTag("Player");
        inputManager = player.GetComponent<NewInputManager>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    protected void Update()
    {
        /*
        if (!grounded)
        {
            return;
        }
        */

        if (player.GetComponent<NewPlayerController>().isDead)
        {
            anim.SetTrigger("Howl");
            return;
        }

        if (canSeePlayer() && !aggro)
        {
            currentState = monsterState.alerted;
        }

        switch (currentState)
        {
            case monsterState.Chasing:
                NavAgent.acceleration = 20;
                NavAgent.speed = 20;
                anim.speed = 1.5f;
                NavAgent.autoBraking = false;
                canSeeWhenCrouched = true;
                anim.SetBool("isChasing", true);
                anim.SetBool("isWandering", false);
                NavAgent.SetDestination(player.transform.position);
                Chasing();
                break;
            case monsterState.Idle:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 2;
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", false);
                anim.speed = 1;
                Idle();
                break;
            case monsterState.wandering:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 2;
                wandering();
                break;
            case monsterState.wait:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 2;
                wait();
                break;
            case monsterState.searching:
                NavAgent.speed = 5;
                NavAgent.acceleration = 5;
                NavAgent.autoBraking = true;
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", true);
                canSeeWhenCrouched = false;
                Searching();
                break;
            case monsterState.alerted:
                NavAgent.speed = 0;
                NavAgent.SetDestination(player.transform.position);
                detectionMeter();
                break;
        }
    }


    protected void detectionMeter()
    {
        if (isInAngle && isInRange && isNotHidden && !aggro)
        {
            Vector3 relativePos = player.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.time * 0.005f);
            audioManager.PlaySFX(Alert, audioManager.playerSFX);

            detectionTimer += Time.deltaTime;
            if (detectionTimer >= DetectionTime)
            {
                detectionTimer = 0;
                aggro = true;
                anim.SetTrigger("Howl");
                audioManager.PlaySFX(howl, MainAudioSrc);
            }
        }
        else if (!isInAngle || !isInRange || !isNotHidden)
        {
            currentState = monsterState.searching;

        }

        if (aggro)
        {
            howlTimer += Time.deltaTime;
            if (howlTimer >= 3)
            {
                howlTimer = 0;
                currentState = monsterState.Chasing;
            }
        }
    }
    /*
    protected void Leap()
    {
        
        leapTarget = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z);
        Vector3 disp = leapTarget - transform.position;
        rb.AddForce(disp.normalized * jumpForce, ForceMode.Impulse);
    }

    
    private void FixedUpdate()
    {
        if (canLeap)
        {
            NavAgent.enabled = false;
            Leap();
            canLeap = false;
        }
        groundCheck();
    }
    */
    protected void Chasing()
    {
        if (NavAgent.remainingDistance <= 15 )
        {
            audioManager.PlaySFX(attack, MainAudioSrc);
            //canLeap = true;
            anim.SetTrigger("Attack");
            NavAgent.speed = 0;
        }
        else NavAgent.speed = 20;


        if (!canSeePlayer())
        {
            currentState = monsterState.searching;

        }
    }
    /*
    protected void groundCheck() 
    {
        RaycastHit hitinfo;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out hitinfo, 1, groundLayerCheck))
        {
            //grounded
            grounded = true;
            NavAgent.enabled = true;

        }
        else
        {
            grounded = false;
            NavAgent.enabled = false;
        }
        Debug.Log(grounded);
        Debug.Log(NavAgent.enabled);
    }
    */
   
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
            aggro = false;
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
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 6, transform.position.z), (player.transform.position - transform.position), out hit, Mathf.Infinity))
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
