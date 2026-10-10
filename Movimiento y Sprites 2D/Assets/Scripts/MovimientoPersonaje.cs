using UnityEngine;

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

    private Rigidbody2D rb;
    private float movimientoHorizontal;
    private bool solicitarSalto;
    private bool estaEnSuelo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
