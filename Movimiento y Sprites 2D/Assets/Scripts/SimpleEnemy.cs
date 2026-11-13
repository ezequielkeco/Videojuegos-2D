
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;

    [Header("Limites de patrullaje")]
    [SerializeField] private Transform puntoIzquierdo;
    [SerializeField] private Transform puntoDerecho;

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

        if (transform.position.x >= puntoDerecho.position.x)
        {
            direccion = -1;
        }
        else if (transform.position.x <= puntoIzquierdo.position.x)
        {
            direccion = 1;
        }

        rb.linearVelocity = new Vector2(
            direccion * velocidad,
            rb.linearVelocity.y
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direccion > 0;
        }
    }
}