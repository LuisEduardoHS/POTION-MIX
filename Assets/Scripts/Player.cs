using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action<float> OnImmunityTick; // Envía el tiempo restante (un float)
    public event System.Action OnImmunityStarted;    // Avisa que empezó
    public event System.Action OnImmunityEnded;      // Avisa que terminó

    public ParticleSystem particulaSalto;

    public float speed = 5f;
    public int vidas = 5;
    private int maxVidas;

    public float runSpeed = 8f;
    private bool isRunning;
    private Rigidbody2D rb2D;

    private float move;

    public float jumpForce = 4f;
    public float reboteForce = 10f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private bool isCrouching = false;
    private bool recibeDamage;

    private bool isImmune = false; // Estado de inmunidad
    public float immunityDuration = 5f;

    private float damageCooldown = 0.5f; // 0.5 segundos de invulnerabilidad
    private float lastDamageTime = -10f;

    public bool muerto;

    private bool hasThrown = false;
    private bool jumpPressed = false;

    private BoxCollider2D playerCollider;
    private Vector2 originalColliderSize;
    private Vector2 crouchColliderSize;

    public GameObject throwablePrefab; // prefab del objeto lanzable
    public Transform launchPoint;      // punto desde donde sale el objeto
    public int throwableCount = 5;     // cuántos puede usar
    public float launchSpeed = 7f;

    [Header("Throw Settings")]
    [Range(0f, 90f)]
    public float throwLaunchAngle = 30f;

    public float throwCooldown = 1.0f; // 1 segundo de espera
    private float lastThrowTime = -10f;

    private Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();

        maxVidas = vidas;

        originalColliderSize = playerCollider.size;
        crouchColliderSize = new Vector2(originalColliderSize.x, originalColliderSize.y / 2);
    }

    void Update()
    {
        animator.SetFloat("movement", Mathf.Abs(move));
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("recibeDamage", recibeDamage);
        animator.SetBool("muerto", muerto);
        animator.SetBool("isCrouching", isCrouching);

        if (muerto) return;

        if (!recibeDamage)
        {
            move = Input.GetAxis("Horizontal");
            isRunning = Input.GetKey(KeyCode.LeftShift); // Shift para correr

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }

            if (isCrouching)
            {
                playerCollider.size = crouchColliderSize;
            }
            else
            {
                playerCollider.size = originalColliderSize;
            }

            if (Input.GetKeyDown(KeyCode.E) && !hasThrown && Time.time > lastThrowTime + throwCooldown)
            {
                lastThrowTime = Time.time; // Reinicia el cronómetro
                animator.SetTrigger("throw");
            }
        }
        else
        {
            move = 0f;
        }

    }



    void LaunchThrowable()
    {
        // Instancia la galleta
        GameObject instancia = Instantiate(throwablePrefab, launchPoint.position, Quaternion.identity);
        ThrowableObject t = instancia.GetComponent<ThrowableObject>();

        // Dirección según donde mira el jugador
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        t.Launch(dir, this.gameObject, throwLaunchAngle);
    }

    public void SpawnThrowable()
    {
        if (throwableCount <= 0 || hasThrown) return; // Evita lanzar más de lo que tienes o múltiples veces en la misma animación

        GameObject obj = Instantiate(throwablePrefab, launchPoint.position, Quaternion.identity);
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        obj.GetComponent<ThrowableObject>().Launch(dir, this.gameObject, throwLaunchAngle);

        throwableCount--;
        hasThrown = true; // Marca que ya lanzamos esta animación
    }

    public void ResetThrow()
    {
        hasThrown = false; // Permite lanzar otra galleta en la próxima animación
    }


    void crearParticulaSalto()
    {
        particulaSalto.Play();
    }

    public void RecibeDamage(Vector2 direccion, int cantDamage)
    {
        if (isImmune) return; // Si eres inmune, ignora todo el daño.

        if (Time.time - lastDamageTime < damageCooldown) return; // Evita daño repetido
        lastDamageTime = Time.time;

        recibeDamage = true;
        vidas -= cantDamage;

        if (vidas <= 0)
        {
            muerto = true;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
            playerCollider.enabled = false;

            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
        else
        {
            float horizontalDirection = Mathf.Sign(transform.position.x - direccion.x);

            rb2D.linearVelocity = Vector2.zero;

            Vector2 forceVector = new Vector2(horizontalDirection * (reboteForce * 0.7f), reboteForce);

            rb2D.AddForce(forceVector, ForceMode2D.Impulse);
        }

        // Desactiva el estado de recibeDamage después de un tiempo corto
        Invoke(nameof(DesactivaDamage), 0.2f); // 0.2 segundos de "stun"
    }

    private void DesactivaDamage()
    {
        recibeDamage = false;
        //rb2D.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (muerto) return;

        if (recibeDamage)
        {
            return;
        }

        // Aplicamos la fisica del movimiento
        float currentSpeed = isRunning ? runSpeed : speed;
        rb2D.linearVelocity = new Vector2(move * currentSpeed, rb2D.linearVelocity.y);

        if ( move != 0 )
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }

        if (jumpPressed && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            crearParticulaSalto();
        }

        jumpPressed = false;

    }

    public void ActivateImmunity()
    {
        // Si ya eres inmune, no hagas nada (o reinicia el timer, como prefieras)
        if (isImmune) return;

        Debug.Log("¡Poción de Inmunidad Activada!");
        StartCoroutine(ImmunityRoutine());
    }

    private System.Collections.IEnumerator ImmunityRoutine()
    {
        // 1. Activa la inmunidad y AVISA a la UI
        isImmune = true;
        OnImmunityStarted?.Invoke();

        float timer = immunityDuration; // (ej. 5 segundos)

        // 2. Bucle de cuenta atrás
        while (timer > 0)
        {
            // Transmite el tiempo restante
            OnImmunityTick?.Invoke(timer);

            // Espera un fotograma
            yield return null;

            // Resta el tiempo que pasó en ese fotograma
            timer -= Time.deltaTime;
        }

        // 3. Desactiva la inmunidad y AVISA a la UI
        isImmune = false;
        OnImmunityEnded?.Invoke();
        Debug.Log("La inmunidad ha terminado.");
    }

    public void Respawn(Vector3 spawnPosition)
    {
        // 1. Mueve al jugador al checkpoint
        transform.position = spawnPosition;

        // 2. Resetea su estado de física y vida
        vidas = maxVidas;
        muerto = false;
        recibeDamage = false;
        isImmune = false; // Resetea el buff de inmunidad

        // 3. Reactiva la física y colisiones
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.linearVelocity = Vector2.zero; // Detiene todo movimiento
        playerCollider.enabled = true;

        // 4. Resetea los temporizadores
        lastDamageTime = -10f;
        lastThrowTime = -10f;

        // 5. Arregla el Animator
        animator.SetBool("muerto", false);
        animator.Play("Idle"); // Fuerza al animador a volver al estado "Idle"

        // 6. Resetea la UI de inmunidad (si existe)
        OnImmunityEnded?.Invoke();
    }
}
