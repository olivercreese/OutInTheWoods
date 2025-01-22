using System.Net;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

///////////////MALUS ARANEA/////////////////////
// This is the AI class for the Malus Aranea enemy
// it inherits from the base class Entity
// it is similar to the Inmunda Formica class but with slight differences 


public class MalusAranea : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject player;
    [SerializeField] AudioClip howl;
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip Alert;
    [SerializeField] AudioSource MainAudioSrc;
    private AudioManager audioManager;
    private NewInputManager inputManager;

    //animation states
    public enum monsterState { Chasing, Idle, wandering, wait, searching,alerted} 
    public monsterState currentState; 
    //Timers
    private float restTimer;
    private float searchTimer;   
    private float detectionTimer;
    private float playerUnseenTimer;
    private float howlTimer;
    //line of sight 
    private bool aggro;
    public float DetectionTime = 3f;
    public float DetectRange = 10f;
    public float DetectAngle = 45f;
    bool isInAngle, isInRange, isNotHidden;
    private bool canSeeWhenCrouched;

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
        if (player.GetComponent<NewPlayerController>().isDead) 
        {
            anim.SetTrigger("Howl"); // plays howl animation on players body when player is dead
            return;
        }

        if (canSeePlayer() && !aggro) {
            currentState = monsterState.alerted; // if the player is seen by the monster, the monster is alerted
        }

        switch (currentState) // state switchboard
        {
            case monsterState.Chasing: 
                NavAgent.acceleration = 20; // changes navagents values for chasing
                NavAgent.speed = 30;
                anim.speed = 1.5f;
                canSeeWhenCrouched = true; // allows the monster to see the player when crouched as they arent seen otherwise
                anim.SetBool("isChasing", true);  // animation booleans
                anim.SetBool("isWandering", false);
                NavAgent.SetDestination(player.transform.position); // sets the destination of the navagent to the player
                Chasing(); //function call for state logic 
                break;
            case monsterState.Idle:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 6; // decreases the detection timer gradually after they have already recently seen the player
                anim.SetBool("isChasing", false); // animation booleans
                anim.SetBool("isWandering", false);
                anim.speed = 1;
                Idle();
                break;
            case monsterState.wandering:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 6;
                wandering();
                break;
            case monsterState.wait:
                if (detectionTimer >= 0) detectionTimer -= Time.deltaTime / 6;
                wait();
                break;
            case monsterState.searching:
                NavAgent.speed = 6;
                NavAgent.acceleration = 6;
                anim.SetBool("isChasing", false);
                anim.SetBool("isWandering", true);
                canSeeWhenCrouched = false; // stops the monster from seeing the player when crouched when the player has been lost
                Searching();
                break;
            case monsterState.alerted:
                NavAgent.speed = 0;
                anim.SetBool("isWandering", false); // enemy stops moving and faces the player
                NavAgent.SetDestination(player.transform.position);
                detectionMeter();
                break;
        }
    }

    protected void detectionMeter()
    {
        if (isInAngle && isInRange && isNotHidden && !aggro) // if line of sight is met, the monster is alerted but not aggroed
        {
            Vector3 relativePos = player.transform.position - transform.position; // makes a slow rotation towards the player when they are seen
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.time * 0.005f);
            audioManager.PlaySFX(Alert, audioManager.playerSFX); // plays the alert sound

            detectionTimer += Time.deltaTime;
            if (detectionTimer >= DetectionTime)  // if the player is seen for a certain amount of time the monster will aggro
            {
                detectionTimer = 0;  // resets the detection timer 
                aggro = true;
                anim.SetTrigger("Howl"); // plays the howl animation
                audioManager.PlaySFX(howl, MainAudioSrc); // plays the howl sound
            }
        }
        else if (!isInAngle || !isInRange || !isNotHidden) // if the player is spotted but not seen then the monster will search for the player
        {
            currentState = monsterState.searching;
            
        }

        if (aggro) 
        {
            howlTimer += Time.deltaTime;
            if (howlTimer >= 3) // counts the time the monster is in the howl animation before chase state is applied
            {
                howlTimer = 0;
                currentState = monsterState.Chasing;
            }
        }
    }

    protected void Chasing()
    {
        if (NavAgent.remainingDistance <= 10)
        {
            anim.SetTrigger("Attack"); // plays the attack animation when the player is within range 
            audioManager.PlaySFX(attack, MainAudioSrc); // plays the attack sound 
            NavAgent.speed = 0; // stops the monster from moving when attacking 
        }
        


        if (!canSeePlayer() && playerUnseen()) // if the player is not seen for longer than 1.5 seconds the monster will search for the player
        {
            currentState = monsterState.searching;
            anim.ResetTrigger("Howl");

        }
        
    }

    protected bool playerUnseen()
    {
        playerUnseenTimer += Time.deltaTime; 
        if (playerUnseenTimer >= 1.5) 
        {
            playerUnseenTimer = 0;
            return true;
        }
        else return false;
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

    protected void Idle() // the idke state plays as a base state for the monster to swap between other non aggro states
    {
        int random = Random.Range(0, 2); // random state switcher for idle to either wander or wait
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
        NavAgent.acceleration = 8; // navagent values for wandering
        NavAgent.speed = 10; 
        NavAgent.autoBraking = true;
        if (NavAgent.remainingDistance <= 0.5f) // if the monster reaches its destination it will go back to idle
        {
            currentState = monsterState.Idle;
            anim.SetBool("isWandering", false);
        }
    }

    protected void Searching()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer >= 10) // if the monster searches for the player for longer than 10 seconds it will go back to idle 
        {
            searchTimer = 0;
            currentState = monsterState.Idle;
            aggro = false; // aggro is set to false
        }


        if (canSeePlayer())
        {
            currentState = monsterState.Chasing; // if the player is seen the monster will chase
        }

        if (NavAgent.destination == null || NavAgent.remainingDistance < 1) NavAgent.SetDestination(RandomNavmeshLocation(5)); // monster searches in the radius of the area it lost the player 
    }


    protected bool canSeePlayer()
    {
        isInAngle = false;
        isInRange = false;
        isNotHidden = false; // sets line of sight booleans to false
        
        if (inputManager.Crouch && !canSeeWhenCrouched) // if the player is crouched the detection range is reduced 
        {
            DetectRange = 10;
        }
        else
        {
            DetectRange = 60;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < DetectRange) isInRange = true; // if the player is within the detection range the monster can see the player

        RaycastHit hit;        
        if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 6,transform.position.z), (player.transform.position - transform.position), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == player) isNotHidden = true; // if the player is not hidden the monster can see the player
        }

        Vector3 side1 = player.transform.position - transform.position; 
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up); // checks the angle between the monsters front vector and the displacement vector between the monster and the player
        if (angle < DetectAngle && angle > -1 * DetectAngle) isInAngle = true; //checks the positive and negative angle to see if the player is within the monsters field of view

        if (isInAngle && isInRange && isNotHidden)
        {
            playerUnseenTimer = 0; //resets the unseen timer when the player is seen
            return true; // if all the conditions are met the player is seen
        }
        else return false;

    } 



}
