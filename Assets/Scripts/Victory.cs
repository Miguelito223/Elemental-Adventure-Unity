using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(true); // Desactiva el objeto al inicio
    }

    public void nextLevel()
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
