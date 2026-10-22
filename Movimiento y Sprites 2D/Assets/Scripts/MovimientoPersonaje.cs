using UnityEngine;
using TMPro;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float controlAereo = 0.5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 9f;
    [SerializeField] private float gravedadNormal = 2.5f;
    [SerializeField] private float multiplicadorCaida = 1.5f;

    [Header("Deteccion del suelo")]
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform puntoSuelo;
    [SerializeField] private float radioDeteccion = 0.15f;

    [Header("Sistema de coleccionables")]
    [SerializeField] private TMP_Text contadorJuego;

    private int objetosRecolectados = 0;
    private int totalObjetos = 6;
    private Rigidbody2D rb;
    private Animator animator;
    private float movimientoHorizontal;
    private bool solicitarSalto;
    private bool estaEnSuelo;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = gravedadNormal;
    }

    void Update()
    {
        movimientoHorizontal = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            movimientoHorizontal = -1f;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            movimientoHorizontal = 1f;
        }

        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(movimientoHorizontal));
        }

        if (spriteRenderer != null)
        {
            if (movimientoHorizontal < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (movimientoHorizontal > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            solicitarSalto = true;
        }
    }

    void FixedUpdate()
    {
        estaEnSuelo = Physics2D.OverlapCircle(
            puntoSuelo.position,
            radioDeteccion,
            capaSuelo
        );

        if (animator != null)
        {
            animator.SetBool("EnSuelo", estaEnSuelo);
        }

        float factorControl = estaEnSuelo ? 1f : controlAereo;

        float velocidadObjetivo =
            movimientoHorizontal * velocidad;

        float nuevaVelocidadX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            velocidadObjetivo,
            velocidad * factorControl * Time.fixedDeltaTime * 10f
        );

        rb.linearVelocity = new Vector2(
            nuevaVelocidadX,
            rb.linearVelocity.y
        );

        if (solicitarSalto && estaEnSuelo)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                fuerzaSalto
            );
        }

        solicitarSalto = false;

        if (rb.linearVelocity.y < 0f && !estaEnSuelo)
        {
            rb.gravityScale =
                gravedadNormal * multiplicadorCaida;
        }
        else
        {
            rb.gravityScale = gravedadNormal;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoSuelo == null) return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            puntoSuelo.position,
            radioDeteccion
        );
    }

    private void ActualizarContador()
    {
        if (contadorJuego != null)
        {
            contadorJuego.text = $"Objetos: {objetosRecolectados} / {totalObjetos}";
        }
    }

    private void Start()
    {
        ActualizarContador();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            objetosRecolectados++;

            ActualizarContador();

            Destroy(other.gameObject);
        }
    }
}
