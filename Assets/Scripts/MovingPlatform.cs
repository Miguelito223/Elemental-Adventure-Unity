using UnityEngine;
using Photon.Pun;

public class MovingPlatform : MonoBehaviourPunCallbacks
{
    public Transform pointA, pointB; // Puntos de destino
    public float speed = 2f; // Velocidad de movimiento

    private Vector3 target; // Objetivo actual

    void Start()
    {
        target = pointB.position; // Comienza moviéndose a pointB
    }

    void Update()
    {
        // Mueve la plataforma hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Si llega al objetivo, cambia de dirección
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform); // Hace que el jugador se mueva con la plataforma
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Evita que el jugador quede pegado a la plataforma al bajarse
        }
    }

}
