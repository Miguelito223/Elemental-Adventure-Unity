using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // Debe ser GameObject en lugar de PhotonView
    public Transform spawnPoint;

    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
        else
        {
            // Instanciación en modo offline
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public override void OnJoinedRoom()
    {
        // Instancia el jugador en red
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
