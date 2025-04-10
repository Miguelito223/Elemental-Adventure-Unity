using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string enemyID; // Identificador único para el enemigo

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
        // Verificar si ya existe un GUID guardado para este enemigo
        if (PlayerPrefs.HasKey(gameObject.name + "_GUID"))
        {
            // Recuperar el GUID guardado
            enemyID = PlayerPrefs.GetString(gameObject.name + "_GUID");
        }
        else
        {
            // Generar un nuevo GUID y guardarlo
            enemyID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(gameObject.name + "_GUID", enemyID);
            PlayerPrefs.Save();
        }

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

        // Verificar si el enemigo ya ha sido derrotado
        if (PlayerPrefs.GetInt(enemyID, 0) == 1)
        {
            // Si el enemigo ya fue derrotado, desactivarlo
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        MoveEnemy();
        DetectEdge(); // Detecta el borde
        DetectWall(); // Detecta paredes
    }

    void DetectWall()
    {
        // Calcular la posición de origen del raycast con el desplazamiento
        Vector2 raycastOrigin = transform.position;
        raycastOrigin.x += isFacingRight ? raycastOffset : -raycastOffset;

        // Raycast para detectar si hay una pared frente al enemigo
        RaycastHit2D wallHit = Physics2D.Raycast(raycastOrigin, isFacingRight ? Vector2.right : Vector2.left, detectionDistance, groundLayer);
        UnityEngine.Debug.DrawRay(raycastOrigin, (isFacingRight ? Vector2.right : Vector2.left) * detectionDistance, Color.blue); // Línea de debug para visualizar el raycast

        // Si el raycast detecta una pared, cambiar la dirección
        if (wallHit.collider != null)
        {
            Flip();
        }
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
            if (groundHit.collider == null)
            {
                Flip();
            }
        }
        else
        {
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
        Defeat();
    }

    public void Defeat()
    {
        // Marcar al enemigo como derrotado
        PlayerPrefs.SetInt(enemyID, 1);
        PlayerPrefs.Save();

        // Desactivar al enemigo
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.LoseLife(); // Sincronizar vidas;
        }
        else if (collision.CompareTag("Bullet"))
        {
            Damage(DamageCount);
        }
    }
}
