using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionDistance = 1f; // Distancia para detectar el borde del precipicio
    private bool isFacingRight = true; // Para saber si el enemigo está mirando hacia la derecha
    public int livesEnemy = 10;
    public int DamageCount = 3;
    public float raycastOffset = 0.5f; // Desplazamiento del raycast desde el centro del
    public LayerMask groundLayer; // Capa del terreno

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            UnityEngine.Debug.LogError("No se ha encontrado el componente Rigidbody2D en el enemigo");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            UnityEngine.Debug.LogError("No se ha encontrado el componente Animator en el enemigo");
        }
    }

    void Update()
    {
        // Si el jugador está a la izquierda del enemigo, cambiar la dirección

        MoveEnemy();
        DetectEdge(); // Detecta el borde
    }

    void MoveEnemy()
    {
        float direction = isFacingRight ? 1f : -1f; // Dirección de movimiento (derecha o izquierda)
        Vector2 movement = Vector2.right * speed * direction * Time.deltaTime;
        transform.Translate(movement); // Mueve al enemigo hacia la derecha o izquierda
        animator.SetBool("walking", movement != Vector2.zero);
    }

    void DetectEdge()
    {
        // Calcular la posición de origen del raycast con el desplazamiento
        Vector2 raycastOrigin = transform.position;
        raycastOrigin.x += isFacingRight ? raycastOffset : -raycastOffset;

        // Raycast para detectar si hay suelo debajo del enemigo
        RaycastHit2D groundHit = Physics2D.Raycast(raycastOrigin, Vector2.down, detectionDistance, groundLayer);
        UnityEngine.Debug.DrawRay(raycastOrigin, Vector2.down * detectionDistance, Color.red); // Línea de debug para visualizar el raycast

        // Si no hay suelo debajo del enemigo, significa que está cerca de un precipicio
        if (groundHit)
        {
            // Si el raycast golpea algo, verificar si es el suelo
            if (groundHit.collider == null)
            {
                UnityEngine.Debug.Log("Flip");
                // Si no hay suelo, cambiar la dirección
                Flip();
            }
            else
            {
                UnityEngine.Debug.Log("Collider: " + groundHit.collider.name);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Flip");
            // Si no hay suelo, cambiar la dirección
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Solo invertir la escala en el eje X
        transform.localScale = localScale;
    }

    void Damage(int Damage)
    {
        livesEnemy -= Damage;
        if (livesEnemy <= 0)
        {
            Kill();
        }
    }

    void Kill()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Llamar a la función para perder vida
            GameManager.instance.LoseLife();
        }
        else if (collision.CompareTag("Bullet"))
        {
            Damage(DamageCount);
        }
    }
}
