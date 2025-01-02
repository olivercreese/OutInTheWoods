using UnityEngine;
using UnityEngine.AI;

public class Sheep : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    

    public enum  sheepState {Grazing,Fleeing,Idle,Resting,Wandering,wait}
    public sheepState currentState;

    public float grazingTime;
    public float RestingTime;
    public float grazeTimer;
    public float restTimer;

    private Vector3 target;
    //public float fleeDistance;
    public float wanderRadius;


    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = sheepState.Idle;
    }
    
    // Update is called once per frame
    void Update()
    {
        //flee mechanic not working
        /*
        if (Vector3.Distance(player.transform.position, transform.position) <= 3f)
        {
            currentState = sheepState.Fleeing;
            anim.SetBool("isFleeing", true);
            anim.SetBool("isGrazing", false);
            anim.SetBool("isResting", false);
            anim.SetBool("isWandering", false);
            grazeTimer = 0;
            restTimer = 0;
        }
        */

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
               /*
             case sheepState.Fleeing:
              NavAgent.speed = 20;
              Fleeing();
              break;
                */
        }


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

    /*
    protected void Fleeing()
    {
        if (NavAgent.destination == null) NavAgent.SetDestination(RandomNavmeshLocation(fleeDistance));

        if (NavAgent.remainingDistance <= 0.5f)
        {
            currentState = sheepState.Idle;
            anim.SetBool("isFleeing", false);
        }
    }
    */

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

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    } //https://discussions.unity.com/t/how-to-get-a-random-point-on-navmesh/73440/2 answer by user:  @Selzier

}
