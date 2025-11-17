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

    // --- VARIABLES DE ATAQUE ---
    [Header("Attack Settings")]
    public GameObject stingerPrefab;
    public Transform stingerLaunchPoint;
    public float attackCooldown = 2.0f;
    private float lastAttackTime = -10f;

    [Header("Drops")]
    public GameObject itemToDropOnDeath;

    void Start()
    {
        Debug.Log("--- ENEMY START ---"); // LOG 1

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startPos = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        // 1. Busca al Player por Tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // 2. Asigna el Transform
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerVivo = true;
            Debug.Log("Player encontrado: " + player.name); // LOG 2
        }
        else
        {
            Debug.LogWarning("¡PLAYER NO ENCONTRADO! Asegúrate de etiquetar (Tag) a tu Player como 'Player'"); // LOG 3
            playerVivo = false;
        }
    }

    void Update()
    {
        // El Animator está desactivado, así que comentamos esto:
        animator.SetBool("InMovement", inMovement);

        // Llamamos a Movimiento() desde Update() para que sea consistente
        Movimiento();
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
        // Si no tenemos jugador (porque no se encontró en Start), no hacer nada.
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        Debug.Log("Movimiento() ejecutándose. Distancia al player: " + distanceToPlayer); // LOG 4

        if (distanceToPlayer < detectionRadius && playerVivo)
        {
            // --- ESTADO: PERSEGUIR Y ATACAR ---
            inMovement = true;

            // 1. LÓGICA HORIZONTAL: Perseguir al jugador en el eje X
            float horizontalDirection = 0;
            if (player.position.x < transform.position.x - 0.5f)
                horizontalDirection = -1;
            else if (player.position.x > transform.position.x + 0.5f)
                horizontalDirection = 1;

            // 2. LÓGICA VERTICAL: Flotar en el sitio
            float y_float = Mathf.Sin(Time.time * idleMoveSpeed + randomOffset) * verticalFloatAmplitude;
            float targetY = startPos.y + y_float;
            float verticalVelocity = (targetY - transform.position.y) * idleMoveSpeed;

            // 3. APLICAR FÍSICA
            rb.linearVelocity = new Vector2(horizontalDirection * speed, verticalVelocity);

            // 4. LÓGICA DE ATAQUE
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Debug.Log("¡Cooldown listo! Intentando atacar..."); // LOG 5
                Attack();
            }

            // 5. VOLTEAR SPRITE
            if (horizontalDirection < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (horizontalDirection > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }
        else if (playerVivo)
        {
            // --- ESTADO: PATRULLAR (IDLE) ---
            float x = Mathf.Sin(Time.time * idleMoveSpeed + randomOffset) * idleMoveRange;
            float y = Mathf.Sin(Time.time * (idleMoveSpeed / 2) + randomOffset) * verticalFloatAmplitude;
            Vector2 targetPos = startPos + new Vector2(x, y);
            Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
            rb.linearVelocity = moveDir * (speed * 0.5f);
            inMovement = true;
            if (moveDir.x < -0.1f)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (moveDir.x > 0.1f)
                transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            inMovement = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Attack()
    {
        Debug.Log("--- Attack() LLAMADO ---"); // LOG 6

        if (player == null)
        {
            Debug.LogError("Player es NULO en Attack()"); // LOG 7
            return;
        }
        if (stingerPrefab == null)
        {
            Debug.LogError("Stinger Prefab es NULO. Asigna el prefab en el Inspector."); // LOG 8
            return;
        }
        if (stingerLaunchPoint == null)
        {
            Debug.LogError("Stinger Launch Point es NULO. Asigna el transform hijo en el Inspector."); // LOG 9
            return;
        }

        lastAttackTime = Time.time;

        Vector2 direction = (player.position - stingerLaunchPoint.position).normalized;
        GameObject stingerObj = Instantiate(stingerPrefab, stingerLaunchPoint.position, Quaternion.identity);
        stingerObj.GetComponent<Stinger>().Launch(direction, GetComponent<Collider2D>());

        Debug.Log("¡AGUIJÓN DISPARADO!"); // LOG 10
    }

    public void RecibeDamage(int damage)
    {
        if (muerto) return;
        vida -= damage;
        if (vida <= 0)
        {
            Muere();
        }
    }

    private void Muere()
    {
        muerto = true;
        inMovement = false;
        rb.linearVelocity = Vector2.zero;
        // animator.SetTrigger("muerto"); // Animator está desactivado
        GetComponent<Collider2D>().enabled = false;

        if (itemToDropOnDeath != null)
        {
            Instantiate(itemToDropOnDeath, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}