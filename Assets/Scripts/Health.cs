using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead => currentHealth <= 0f;

    public UnityEvent onHit;
    public UnityEvent onDeath;
    public void Hit(float damage = 1f)
    {
        onHit?.Invoke();
        currentHealth -= damage;

        if(currentHealth <= 0f)
        {
            currentHealth = 0f;
            onDeath?.Invoke();
            print("Dead " + gameObject.name);
        }
    }
}
