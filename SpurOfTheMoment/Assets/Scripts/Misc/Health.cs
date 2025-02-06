using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 1;
    protected int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); // Default behavior: Destroy on death
    }
}
