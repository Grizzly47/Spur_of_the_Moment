using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private int maxBounces = 3;
    private int bounceCount = 0;
    private float bulletSpeed;
    private bool isFrozen = false;
    private Vector2 storedVelocity;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameManager.Instance.RegisterBullet(this);
    }

    public void SetInitialSpeed(float speed)
    {
        bulletSpeed = speed;
        rb.linearVelocity = rb.linearVelocity.normalized * bulletSpeed;
    }

    public void SetFrozen(bool freeze)
    {
        isFrozen = freeze;
        if (isFrozen)
        {
            storedVelocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero; // Stop movement
        }
        else
        {
            rb.linearVelocity = storedVelocity; // Resume movement
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterBullet(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayerMask) != 0)
        {
            if (bounceCount < maxBounces)
            {
                bounceCount++;
                Vector2 normal = collision.GetContact(0).normal;
                rb.linearVelocity = (rb.linearVelocity - 2 * Vector2.Dot(rb.linearVelocity, normal) * normal).normalized * bulletSpeed;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
