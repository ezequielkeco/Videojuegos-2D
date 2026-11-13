
using UnityEngine;

public class ComplexEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float velocidadPersecucion = 3.5f;

    [Header("Patrullaje")]
    [SerializeField] private Transform puntoIzquierdo;
    [SerializeField] private Transform puntoDerecho;

    [Header("Deteccion del personaje")]
    [SerializeField] private Transform personaje;
    [SerializeField] private float rangoDeteccion = 4f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int direccion = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (puntoIzquierdo == null || puntoDerecho == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float limiteIzquierdo = Mathf.Min(
            puntoIzquierdo.position.x,
            puntoDerecho.position.x
        );

        float limiteDerecho = Mathf.Max(
            puntoIzquierdo.position.x,
            puntoDerecho.position.x
        );

        float posicionX = rb.position.x;

        float tolerancia = 0.05f;

        float velocidadActual = velocidad;

        bool detectarPersonaje = personaje != null &&
            Vector2.Distance(
                rb.position,
                personaje.position
            ) <= rangoDeteccion;

        if (detectarPersonaje)
        {
            float diferenciaX =
                personaje.position.x - posicionX;

            if (Mathf.Abs(diferenciaX) > 0.1f)
            {
                direccion = diferenciaX > 0f ? 1 : -1;
                velocidadActual = velocidadPersecucion;
            }
            else
            {
                velocidadActual = 0f;
            }
        }
        else
        {
            if (direccion > 0 &&
                posicionX >= limiteDerecho - tolerancia)
            {
                direccion = -1;
            }
            else if (direccion < 0 &&
                     posicionX <= limiteIzquierdo + tolerancia)
            {
                direccion = 1;
            }
        }

        float velocidadX = direccion * velocidadActual;

        if (velocidadX > 0f)
        {
            float distanciaDisponible =
                limiteDerecho - posicionX;

            velocidadX = Mathf.Min(
                velocidadX,
                Mathf.Max(
                    0f,
                    distanciaDisponible / Time.fixedDeltaTime
                )
            );
        }
        else if (velocidadX < 0f)
        {
            float distanciaDisponible =
                posicionX - limiteIzquierdo;

            velocidadX = Mathf.Max(
                velocidadX,
                -Mathf.Max(
                    0f,
                    distanciaDisponible / Time.fixedDeltaTime
                )
            );
        }

        rb.linearVelocity = new Vector2(
            velocidadX,
            rb.linearVelocity.y
        );

        Debug.Log(
    $"Posición: {posicionX:F2} | " +
    $"Izquierda: {limiteIzquierdo:F2} | " +
    $"Derecha: {limiteDerecho:F2} | " +
    $"Dirección: {direccion} | " +
    $"Velocidad X: {velocidadX:F2}"
);

        if (spriteRenderer != null && velocidadX != 0f)
        {
            spriteRenderer.flipX = direccion > 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            rangoDeteccion
        );
    }
}