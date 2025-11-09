using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 8.0f;
    public float speed = 2.0f;
    public float idleMoveRange = 1.5f; // Amplitud del vuelo aleatorio
    public float idleMoveSpeed = 1.5f; // Velocidad del vuelo aleatorio
    public float verticalFloatAmplitude = 0.5f; // Movimiento vertical (flotar)

    private bool muerto;
    public int vida = 1;

    private Rigidbody2D rb;
    private Animator animator;

    private bool playerVivo = true;
    private bool inMovement;
    private Vector2 startPos;
    private float randomOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startPos = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // desfase aleatorio del movimiento senoidal
    }

    void Update()
    {
        if (playerVivo)
        {
            Movimiento();
        }

        animator.SetBool("InMovement", inMovement);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDamage = new Vector2(transform.position.x, transform.position.y);
            Player playerScript = collision.gameObject.GetComponent<Player>();

            playerScript.RecibeDamage(direccionDamage, 1);
            playerVivo = !playerScript.muerto;

            if (!playerVivo)
            {
                inMovement = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            // Movimiento hacia el jugador (en vuelo)
            Vector2 direction = (player.position - transform.position).normalized;

            rb.linearVelocity = direction * speed;
            inMovement = true;

            // Voltea segun direccion
            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // Vuelo libre (patrullaje)
            float x = Mathf.Sin(Time.time * idleMoveSpeed + randomOffset) * idleMoveRange;
            float y = Mathf.Sin(Time.time * (idleMoveSpeed / 2) + randomOffset) * verticalFloatAmplitude;

            Vector2 targetPos = startPos + new Vector2(x, y);
            Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;

            rb.linearVelocity = moveDir * (speed * 0.5f); // Mas lento cuando patrulla
            inMovement = true;

            // Voltea segun movimiento lateral
            if (moveDir.x < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (moveDir.x > 0.1f)
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void RecibeDamage(int damage)
    {
        if (muerto) return; // evita recibir más daño si ya murió

        vida -= damage;

        // Activa animación de daño si tienes una
        //animator.SetTrigger("Hit");

        if (vida <= 0)
        {
            Muere();
        }
    }

    private void Muere()
    {
        muerto = true;
        inMovement = false;
        rb.linearVelocity = Vector2.zero; // detiene su movimiento

        animator.SetTrigger("muerto"); // activa animación de muerte (si la tienes)

        // Desactiva colisiones para que no interfiera
        GetComponent<Collider2D>().enabled = false;

        // Destruye el enemigo después de un pequeño retraso
        Destroy(gameObject, 1f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
