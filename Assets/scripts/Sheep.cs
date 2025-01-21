using UnityEngine;
using UnityEngine.AI;

public class Sheep : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] sheepNoises;
    private AudioManager audioManager;

    public enum  sheepState {Grazing,Fleeing,Idle,Resting,Wandering,wait}
    public sheepState currentState;

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
 

        switch (currentState)
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
        grazeTimer += Time.deltaTime;
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
        if (soundTimer >= Random.Range(8, 15))
        {
            soundTimer = 0;
            audioManager.PlaySFX(sheepNoises[Random.Range(0, sheepNoises.Length)], audioSource);
        }

    }

    protected void Idle()
    {
        int randomValue = Random.Range(1, 7);
        if (randomValue != 2) anim.SetBool("isResting", false);
        switch (randomValue)
        {
            case 1:
                currentState = sheepState.Grazing;
                anim.SetBool("isGrazing", true);
                break;
            case 2:
                currentState = sheepState.Resting;
                anim.SetBool("isResting", true);
                break;
            case 3:
                currentState = sheepState.wait;
                break;
            default:
                currentState = sheepState.Wandering;
                anim.SetBool("isWandering", true);
                NavAgent.SetDestination(RandomNavmeshLocation(wanderRadius));
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
