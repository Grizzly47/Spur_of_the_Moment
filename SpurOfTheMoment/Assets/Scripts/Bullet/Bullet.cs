using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private int maxBounces = 3;
    private int bounceCount = 0;
    private float bulletSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetInitialSpeed(float speed)
    {
        bulletSpeed = speed;
        rb.linearVelocity = rb.linearVelocity.normalized * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayerMask) == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        bounceCount++;

        // Get collision normal
        Vector2 normal = collision.GetContact(0).normal;

        // Reflect velocity and maintain speed
        rb.linearVelocity = (rb.linearVelocity - 2 * Vector2.Dot(rb.linearVelocity, normal) * normal).normalized * bulletSpeed;
    }
}
