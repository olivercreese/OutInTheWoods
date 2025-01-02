using UnityEngine;

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



}
