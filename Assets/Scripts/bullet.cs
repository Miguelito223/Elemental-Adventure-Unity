using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Destruir la bala después de 5 segundos si no choca con nada
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().LoseLife();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destruir después de 5 segundos
        }
    }

    public void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().LoseLife();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            int damageCount = collision.gameObject.GetComponent<Enemy>().DamageCount;
            collision.gameObject.GetComponent<Enemy>().Damage(damageCount);
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destruir después de 5 segundos
        }
    }
}
