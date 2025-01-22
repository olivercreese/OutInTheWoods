using UnityEngine;
using UnityEngine.AI;
/////SHEEP/////
// This class is the AI for the Sheep enemy
// it inherits from the base class Entity
public class Sheep : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] sheepNoises; // array of sheep noises
    private AudioManager audioManager;
    //animation
    public enum  sheepState {Grazing,Fleeing,Idle,Resting,Wandering,wait}
    public sheepState currentState;
    //Timers
    private float grazingTime;
    private float RestingTime;
    private float grazeTimer;
    private float restTimer;
    private float soundTimer;
    //public float fleeDistance;
    public float wanderRadius;


    protected void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = sheepState.Idle;
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }
    
    // Update is called once per frame
    protected void Update()
    {
 

        switch (currentState) //switch statement for the sheep states 
        {
            case sheepState.Grazing:
                Grazing();
                break;
            case sheepState.Idle:
                Idle();
                break;
            case sheepState.Resting:
                Resting();
                break;
            case sheepState.Wandering:
                NavAgent.speed = 5;
                Wandering();
                break;
            case sheepState.wait:
                idleWait();
                break;
   
        }
        SheepSounds();

    }

    protected void Grazing()
    {
        grazeTimer += Time.deltaTime; //plays the grazing animation for a set amount of time
        if (grazeTimer >= grazingTime)
        {
            grazeTimer = 0;
            currentState = sheepState.Idle;
            anim.SetBool("isGrazing",false);
        }
    }

    protected void SheepSounds()
    {
        soundTimer += Time.deltaTime;
        int randomValue = Random.Range(8, 60); 
        if (soundTimer >= randomValue) // at random intervals the sheep will play a random noise from the array
        {
            soundTimer = 0;
            audioManager.PlaySFX(sheepNoises[Random.Range(0, sheepNoises.Length)], audioSource);
        }

    }

    protected void Idle() // the idle state plays as a base state for the sheep to swap between other states
    {
        int randomValue = Random.Range(1, 7);
        if (randomValue != 2) anim.SetBool("isResting", false); // avoids the sheep getting up from resting then going back to resting
        switch (randomValue)
        {
            case 1:
                currentState = sheepState.Grazing;
                anim.SetBool("isGrazing", true); //plays the grazing animation
                break;
            case 2:
                currentState = sheepState.Resting;
                anim.SetBool("isResting", true); //plays the resting animation
                break;
            case 3:
                currentState = sheepState.wait;
                break;
            default:
                currentState = sheepState.Wandering;
                anim.SetBool("isWandering", true); //plays the wandering animation
                NavAgent.SetDestination(RandomNavmeshLocation(wanderRadius)); //sets the destination to a random location in the wander radius
                break;
        }
        
    }

    protected void idleWait()
    {
        restTimer += Time.deltaTime;
        if (restTimer >= Random.Range(3,10))
        {
            restTimer = 0;
            currentState = sheepState.Idle;
        }
    }
    protected void Resting()
    {
        restTimer += Time.deltaTime;
        if (restTimer >= RestingTime)
        {
            restTimer = 0;
            currentState = sheepState.Idle;
        }
    }
    protected void Wandering()
    {
        if (NavAgent.remainingDistance <= 0.5f)
        {
            currentState = sheepState.Idle;
            anim.SetBool("isWandering", false);
        }
    }


}
