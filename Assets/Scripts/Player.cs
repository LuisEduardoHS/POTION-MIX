using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleSystem particulaSalto;

    public float speed = 5f;
    public int vidas = 3;

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

    private Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();

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

            if(Input.GetKeyDown(KeyCode.E) && !hasThrown)
            {
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

        // Lanzamiento: pasas dirección y el jugador (this.gameObject)
        t.Launch(dir, this.gameObject); // opcional: usa el ángulo por defecto 30f
    }

    public void SpawnThrowable()
    {
        if (throwableCount <= 0 || hasThrown) return; // Evita lanzar más de lo que tienes o múltiples veces en la misma animación

        GameObject obj = Instantiate(throwablePrefab, launchPoint.position, Quaternion.identity);
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        obj.GetComponent<ThrowableObject>().Launch(dir, this.gameObject);

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
}
