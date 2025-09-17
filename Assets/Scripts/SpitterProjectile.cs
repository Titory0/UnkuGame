using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    public float projectileSpeed;

    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = transform.up * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) { return; }

        if (collision.CompareTag("Player"))
        {
            // Make player take damage
        }
        if(collision.CompareTag("Untagged"))
        {
            print(collision.name);
            transform.GetChild(0).parent = null;
            Destroy(gameObject);
        }
    }
}
