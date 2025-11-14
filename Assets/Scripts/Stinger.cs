using UnityEngine;

public class Stinger : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;

    private Rigidbody2D rb;
    private Vector2 launchDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 4f);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = launchDirection * speed;
        }
    }

    public void Launch(Vector2 direction, Collider2D creatorCollider)
    {
        launchDirection = direction;

        // Ignora la colisión con el enemigo que lo disparó
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null && creatorCollider != null)
        {
            Physics2D.IgnoreCollision(myCollider, creatorCollider);
        }

        // Rotar el sprite
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 45f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if(playerScript != null && !playerScript.muerto)
            {
                playerScript.RecibeDamage(transform.position, damage);
            }
        }

        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
