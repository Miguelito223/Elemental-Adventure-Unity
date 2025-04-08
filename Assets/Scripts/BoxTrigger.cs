using UnityEngine;
using UnityEngine.SceneManagement;

// Renombrar la clase para evitar conflictos de nombres  
public class BoxTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador toca el objeto  
        {
            GameManager.instance.Victory(); // Llama al método de victoria  
        }
    }
}
