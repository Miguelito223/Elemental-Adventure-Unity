using UnityEngine;
using UnityEngine.SceneManagement;

public class Box : MonoBehaviour
{
    public string nextSceneName; // Nombre de la escena a cargar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador toca el objeto
        {
            SceneManager.LoadScene(nextSceneName); // Carga la escena especificada
        }
    }
}
