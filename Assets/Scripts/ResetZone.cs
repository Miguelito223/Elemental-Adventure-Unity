using UnityEngine;

public class ResetZone : MonoBehaviour
{
    public Transform respawnPoint; // Punto donde reaparece el jugador

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            collision.transform.position = respawnPoint.position; // Mueve al jugador al punto de reinicio
        }
    }
}
