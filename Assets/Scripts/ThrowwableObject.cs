using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public float speed = 15f; // velocidad del lanzamiento
    public Rigidbody2D rb;
    public int damage = 1;   // daño al enemigo

    private Vector2 direction;

    void Start()
    {
        // El Rigidbody debe estar asignado
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir, GameObject thrower, float angle = 30f)
    {
        // Ignorar colisión con quien la lanzó
        Collider2D throwerCollider = thrower.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();

        if (throwerCollider != null && myCollider != null)
            Physics2D.IgnoreCollision(myCollider, throwerCollider);

        // Dirección diagonal según el ángulo
        float rad = angle * Mathf.Deg2Rad;
        Vector2 launchDir = new Vector2(Mathf.Cos(rad) * Mathf.Sign(dir.x), Mathf.Sin(rad));

        rb.linearVelocity = launchDir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión detectada con: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("¡Golpeó al enemigo!");

            Enemy enemigo = collision.gameObject.GetComponent<Enemy>();
            if (enemigo != null)
            {
                Debug.Log("Se encontró el script Enemy, aplicando daño...");
                enemigo.RecibeDamage(damage);
            }
            else
            {
                Debug.LogWarning("El enemigo no tiene el script 'Enemy'.");
            }
        }

        Destroy(gameObject);
    }
}
