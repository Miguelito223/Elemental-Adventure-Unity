using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using Photon.Pun;

public class ResetZone : MonoBehaviour
{
    public Transform respawnPoint; // Punto donde reaparece el jugador

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            Debug.Log("¡Jugador ha tocado el trigger!");
            GameManager.instance.LoseLife();
            collision.transform.position = respawnPoint.position; // Mueve al jugador al punto de reinicio
        }
    }
}
