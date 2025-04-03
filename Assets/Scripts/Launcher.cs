using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PhotonView playerprefab;
    public Transform Spawnpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject player =  PhotonNetwork.Instantiate(playerprefab.name, Spawnpoint.position, Spawnpoint.rotation);
    }
}
