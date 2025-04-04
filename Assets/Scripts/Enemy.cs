using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionDistance = 1f; // Distancia para detectar el borde del precipicio
    public Transform player;  // Asignar manualmente el jugador en el Inspector
    private bool isFacingRight = true; // Para saber si el enemigo está mirando hacia la derecha
    public int livesEnemy = 10;
    public int DamageCount = 3;
    public float groundCheckDistance = 1f; // Distancia para verificar el suelo debajo del enemigo

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("No se ha asignado el jugador al enemigo");
        }
    }

    void Update()
    {
        if (player == null) return;  // Si no hay jugador asignado, no hacer nada

        MoveEnemy();
        DetectEdge(); // Detecta el borde
    }

    void MoveEnemy()
    {
        float direction = isFacingRight ? 1f : -1f; // Dirección de movimiento (derecha o izquierda)
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime); // Mueve al enemigo hacia la derecha o izquierda

        // Detectar el borde y cambiar de dirección si el enemigo llega a un precipicio o pared
        if (isFacingRight)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance);
            if (groundHit.collider == null) // Si no hay suelo
            {
                Flip(); // Cambiar de dirección
            }
        }
        else
        {
            RaycastHit2D edgeHit = Physics2D.Raycast(transform.position, Vector2.left, detectionDistance);
            if (edgeHit.collider == null) // Si no hay pared
            {
                Flip(); // Cambiar de dirección
            }
        }
    }

    void DetectEdge()
    {
        // Raycast para detectar si hay suelo debajo del enemigo
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance);

        // Si no hay suelo debajo del enemigo, significa que está cerca de un precipicio
        if (groundHit.collider == null)
        {
            Flip(); // Cambiar la dirección
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
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
