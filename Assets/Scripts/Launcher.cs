using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // Prefab del jugador
    public Transform spawnPoint;

    void Start()
    {

        // Verificar si el juego está en modo offline
        if (PhotonNetwork.OfflineMode)
        {
            Debug.Log("Modo offline activado. Instanciando jugador localmente...");
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation); // Instanciar jugador localmente
        }
        else
        {
            // Conectar al servidor de Photon
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("No estás en una sala. No se puede instanciar el objeto.");
            }

        }

    }
}
