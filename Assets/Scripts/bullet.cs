using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start()
    {
        // Destruir la bala después de 5 segundos si no choca con nada
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.LoseLife();
        }
       
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.LoseLife();
        }

        Destroy(gameObject);
    }
}
