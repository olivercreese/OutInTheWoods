using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{

    public int maxHealth;
    public float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Heal(float amount)
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
