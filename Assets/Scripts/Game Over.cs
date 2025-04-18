using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameOver : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true); // Desactiva el objeto al inicio
    }

    public void reloadLevel()
    {
        int level = PlayerPrefs.GetInt("Level", 3); // Obtener el nivel guardado, por defecto 3
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.LoadLevel(level); // Sincronizar nivel en red
        }
        else
        {
            SceneManager.LoadScene(level); // Cargar la escena de victoria localmente
        }
    }
}
