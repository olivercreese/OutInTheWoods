using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{

    public int maxHealth;
    protected int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }


    public virtual bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
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
