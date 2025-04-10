using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance; // Patrón Singleton para acceder desde otros scripts
    public GameObject gameOverPanel; // Arrastra el panel de Game Over aquí
    public GameObject victoryPanel; // Arrastra el panel de Victoria aquí

    public int coins; // Monedas actuales
    public int lives; // Vidas actuales
    public int level; 

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    private bool isInvulnerable = false; // Controla si el jugador está en estado de invulnerabilidad
    public float invulnerabilityDuration = 2f; // Duración del estado de invulnerabilidad en segundos

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // Cargar vidas, monedas y nivel desde PlayerPrefs
        LoadGameData();

        // Inicializa el juego
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        UpdateUI();
    }

    public void AddCoin()
    {
        coins++;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (isInvulnerable) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOverRPC();
        }
        else
        {
            StartCoroutine(InvulnerabilityTimer());
        }
    }


    private IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true; // Activa el estado de invulnerabilidad
        yield return new WaitForSeconds(invulnerabilityDuration); // Espera el tiempo especificado
        isInvulnerable = false; // Desactiva el estado de invulnerabilidad
    }

    [PunRPC]
    void GameOver()
    {
        // Mostrar el panel de Game Over
        gameOverPanel.SetActive(true);
        // Detener el juego o hacer que el jugador no pueda moverse, por ejemplo:
        Time.timeScale = 0f; // Detiene el tiempo
    }

    [PunRPC]
    public void Victory()
    {
        // Guardar vidas, monedas y nivel en PlayerPrefs
        SaveGameData();

        // Mostrar el panel de Victoria
        victoryPanel.SetActive(true);
        // Detener el tiempo del juego
        Time.timeScale = 0f;
    }

    [PunRPC]
    public void Retry()
    {
        // Restablecer las vidas a 3
        lives = 3;

        // Actualizar la UI para reflejar las nuevas vidas
        UpdateUI();

        // Guardar los datos actualizados
        SaveGameData();

        // Recargar la escena actual
        SceneManager.LoadScene(level);
        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    [PunRPC]
    public void Next_Level()
    {
        // Incrementar el nivel antes de guardar
        level++;

        // Verificar si el siguiente nivel existe
        if (level >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("No hay más niveles. Recargando el nivel actual.");
            level--; // Restablecer el nivel al actual
            SaveGameData();

            // Cargar el nivel actual
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LoadLevel(level); // Cargar el siguiente nivel en red
            }
            else
            {
                SceneManager.LoadScene(level); // Cargar el siguiente nivel localmente
            }
        }
        else
        {
            // Guardar el nuevo nivel, vidas y monedas en PlayerPrefs
            SaveGameData();

            // Cargar el siguiente nivel
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LoadLevel(level); // Cargar el siguiente nivel en red
            }
            else
            {
                SceneManager.LoadScene(level); // Cargar el siguiente nivel localmente
            }
        }

        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    public void Next_LevelRPC()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("Next_Level", RpcTarget.All);
        }
        else
        {
            Next_Level(); // Llamar al método localmente si está en modo offline
        }
    }

    public void RetryRPC()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("Retry", RpcTarget.All);
        }
        else
        {
            Retry(); // Llamar al método localmente si está en modo offline
        }
    }

    public void GameOverRPC()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("GameOver", RpcTarget.All);
        }
        else
        {
            GameOver(); // Llamar al método localmente si está en modo offline
        }
    }

    public void VictoryRPC()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("Victory", RpcTarget.All);
        }
        else
        {
            Victory(); // Llamar al método localmente si está en modo offline
        }
    }


    public void LoadMainMenu()
    {
        // Guardar vidas y monedas en PlayerPrefs
        SaveGameData();

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // Desconectar del multijugador
        }

        // Cargar el menú principal
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Enviar el estado actual de vidas y monedas al nuevo jugador
        photonView.RPC("SyncGameState", newPlayer, coins, lives);
    }

    [PunRPC]
    public void SyncGameState(int syncedCoins, int syncedLives)
    {
        coins = syncedCoins;
        lives = syncedLives;
        UpdateUI();
    }

    void UpdateUI()
    {
        coinText.text = "Monedas: " + coins;
        livesText.text = "Vidas: " + lives;
    }

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Lives", lives); // Guardar vidas
        PlayerPrefs.SetInt("Coins", coins); // Guardar monedas
        PlayerPrefs.SetInt("Level", level);

        PlayerPrefs.Save(); // Asegurarse de que los datos se guarden

    }

    public void LoadGameData()
    {
        // Cargar vidas, monedas y nivel desde PlayerPrefs
        lives = PlayerPrefs.GetInt("Lives", 3); // Si no hay datos guardados, usa 3 como valor predeterminado
        coins = PlayerPrefs.GetInt("Coins", 0); // Si no hay datos guardados, usa 0 como valor predeterminado
        level = PlayerPrefs.GetInt("Level", 1); // Si no hay datos guardados, usa el nivel 1 como predeterminado
    }

    private void OnApplicationQuit()
    {
        // Guardar los datos cuando se cierre el juego
        SaveGameData();
    }
    public void ResetCoins()
    {
        // Eliminar todos los datos de monedas guardados
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("energyball"))
        {
            PlayerPrefs.DeleteKey(coin.GetComponent<EnergyBall>().EnergyID);
        }

        PlayerPrefs.Save();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar datos desde el cliente local
            stream.SendNext(lives);
            stream.SendNext(coins);
            stream.SendNext(level);
        }
        else
        {
            // Recibir datos desde otros clientes
            lives = (int)stream.ReceiveNext();
            coins = (int)stream.ReceiveNext();
            level = (int)stream.ReceiveNext();

            // Actualizar la UI después de recibir los datos
            UpdateUI();
        }
    }
}
