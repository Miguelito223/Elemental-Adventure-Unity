using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviourPunCallbacks
{
    public Button button;
    public Button button2;
    public SettingMenu settingmenu;

    private void Start()
    {
        gameObject.SetActive(true);
    }
    public void StartGame()
    {
        PhotonNetwork.OfflineMode = true;
        int savedLevel = PlayerPrefs.GetInt("Level", 3); // Cargar el nivel guardado
        SceneManager.LoadScene(savedLevel);
    }
    public void StartMultiplayerGame()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al Master Server. Intentando unirse o crear una sala...");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Unido a la sala.");

        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogError("PhotonNetwork.CurrentRoom es null. No se puede acceder a las propiedades de la sala.");
            return;
        }

        // Si no eres el maestro, carga el nivel desde las propiedades de la sala
        if (!PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Level"))
            {
                int level = (int)PhotonNetwork.CurrentRoom.CustomProperties["Level"];
                Debug.Log($"Cargando nivel {level} desde las propiedades de la sala.");
                PhotonNetwork.LoadLevel(level);
            }
            else
            {
                Debug.LogError("La propiedad 'Level' no existe en las propiedades de la sala.");
            }
        }
        else
        {
            Debug.Log("Sala creada correctamente.");
            int savedLevel = PlayerPrefs.GetInt("Level", 3); // Cargar el nivel guardado
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Level", savedLevel } });
            Debug.Log($"Propiedad 'Level' establecida en {savedLevel}.");
            PhotonNetwork.LoadLevel(savedLevel); // Usar Photon para cargar el nivel sincronizado
        }
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"No se pudo unir a una sala aleatoria: {message}. Creando una nueva sala...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public void SettingMenu()
    {
        gameObject.SetActive(false);
        settingmenu.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
