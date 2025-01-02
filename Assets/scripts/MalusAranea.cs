using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static Sheep;

public class MalusAranea : Entity
{
    [SerializeField] NavMeshAgent NavAgent;
    [SerializeField] Animator anim;
    [SerializeField] GameObject player;

    public enum monsterState { Chasing, Idle, Attacking, wandering, wait }
    public monsterState currentState;

    private float restTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = monsterState.Idle;
    }

    // Update is called once per frame
    protected void Update()
    {
        switch (currentState)
        {
            case monsterState.Chasing:
                Chasing();
                anim.SetBool("isChasing", true);
                NavAgent.SetDestination(player.transform.position);
                break;
            case monsterState.Idle:
                Idle();
                break;
            case monsterState.Attacking:
                anim.SetBool("isAttacking", true);
                Attacking();
                break;
            case monsterState.wandering:
                wandering();
                break;
            case monsterState.wait:
                wait();
                break;
        }
    }

    protected void Chasing()
    {
        if (NavAgent.remainingDistance <= 1)
        {
            currentState = monsterState.Attacking;
            anim.SetBool("isChasing", false);
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
    protected void Attacking()
    {
        
    }
    protected void wandering()
    {
        if (NavAgent.remainingDistance <= 0.5f)
        {
            currentState = monsterState.Idle;
            anim.SetBool("isWandering", false);
        }
    }

}
