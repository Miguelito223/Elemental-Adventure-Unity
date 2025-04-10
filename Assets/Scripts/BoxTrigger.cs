using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

// Renombrar la clase para evitar conflictos de nombres  
public class BoxTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador toca el objeto  
        {
            GameManager.instance.VictoryRPC(); // Llama al método de victoria  
        }
    }
}
