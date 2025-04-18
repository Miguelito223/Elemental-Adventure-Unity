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

    void FixedUpdate()
    {
        // Mueve la plataforma hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Si llega al objetivo, cambia de dirección
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            collision.transform.SetParent(transform);

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            collision.transform.SetParent(null);

        }
    }

}
