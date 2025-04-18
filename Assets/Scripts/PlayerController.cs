using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public SpriteRenderer spriterender;
    public Animator animator;
    public Rigidbody2D RB2d;
    public AudioSource audiosource;
    public Transform bulletPos;
    public Transform bulletSpawn;
    public GameObject bullet;
    public Camera playercamera;
    public GameObject lifespanel;
    public GameObject coinspanel;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    bool canjump;
    bool isinground;
    public float speed = 100f; // Ajusta la velocidad según sea necesario
    public float bulletCooldown = 1f; // Tiempo de espera entre disparos (1 segundo)
    public float bulletSpeed = 10f;
    bool canShoot = true;

    private Vector2 platformVelocity;

    public int coins; // Monedas actuales
    public int lives; // Vidas actuales
    public int level;

    private bool isInvulnerable = false; // Controla si el jugador está en estado de invulnerabilidad
    public float invulnerabilityDuration = 2f; // Duración del estado de invulnerabilidad en segundos

    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine)
            {
                playercamera.enabled = false;
                coinspanel.SetActive(false);
                lifespanel.SetActive(false);
                return;
            }
        }

        audiosource = GetComponent<AudioSource>();
        RB2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();

        playercamera.enabled = true; // Habilitar la cámara del jugador

        PhotonNetwork.AutomaticallySyncScene = true;
        LoadGameData();
        UpdateUI();
    }

    void Update()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine)
            {
                playercamera.enabled = false;
                coinspanel.SetActive(false);
                lifespanel.SetActive(false);
                return;
            }
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        RB2d.AddForce(new Vector2(moveInput * speed * Time.deltaTime, 0));

        animator.SetBool("moving", moveInput != 0);

        // Actualizar el giro del sprite localmente
        if (moveInput != 0)
        {
            spriterender.flipX = moveInput < 0;
        }

        if (!audiosource.isPlaying && isinground && moveInput != 0) audiosource.Play();
        else if ((moveInput == 0 || !isinground) && audiosource.isPlaying) audiosource.Pause();

        if (Input.GetKeyDown(KeyCode.Space) && canjump)
        {
            canjump = false;
            isinground = false;
            animator.SetBool("jumping", true);
            RB2d.AddForce(new Vector2(0, speed));
        }

        // Calcular la dirección hacia el mouse (esto ya no afecta al jugador)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - bulletPos.position).normalized;

        if (Input.GetKeyDown(KeyCode.S) && canShoot)
        {
            StartCoroutine(Shoot(direction));
        }

        if (RB2d.linearVelocity.y < -0.1f)
        {
            animator.SetBool("falling", true);
            animator.SetBool("jumping", false);
        }
        else if (RB2d.linearVelocity.y == 0)
        {
            animator.SetBool("falling", false);
        }
    }

    [PunRPC]
    private IEnumerator Shoot(Vector2 direction)
    {
        canShoot = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletPos.transform.rotation = Quaternion.Euler(0, 0, angle);

        GameObject bulletInstance;

        if (PhotonNetwork.IsConnected)
        {
            bulletInstance = PhotonNetwork.Instantiate(bullet.name, bulletSpawn.position, bulletSpawn.rotation);
        }
        else
        {
            bulletInstance = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        }

        Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();

        bulletRb.linearVelocity = direction * bulletSpeed;

        yield return new WaitForSeconds(bulletCooldown);

        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        isinground = true;
        canjump = true;
        animator.SetBool("jumping", false);
        animator.SetBool("falling", false);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        isinground = false;

    }

    // Sincronizar datos con Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("Enviando");
            // Enviar el estado de las variables
            stream.SendNext(spriterender.flipX);
            stream.SendNext(canjump);
            stream.SendNext(isinground);
            stream.SendNext(canShoot);
            stream.SendNext(audiosource.isPlaying);
            stream.SendNext(bulletSpeed);
            stream.SendNext(bulletCooldown);
            stream.SendNext(speed);
            stream.SendNext(lives);
            stream.SendNext(coins);
            stream.SendNext(level);
        }
        else
        {
            Debug.Log("Recibiendo");
            try
            {
                spriterender.flipX = (bool)stream.ReceiveNext();
                canjump = (bool)stream.ReceiveNext();
                isinground = (bool)stream.ReceiveNext();
                canShoot = (bool)stream.ReceiveNext();
                audiosource.enabled = (bool)stream.ReceiveNext();
                bulletSpeed = (float)stream.ReceiveNext();
                bulletCooldown = (float)stream.ReceiveNext();
                speed = (float)stream.ReceiveNext();
                lives = (int)stream.ReceiveNext();
                coins = (int)stream.ReceiveNext();
                level = (int)stream.ReceiveNext();
            }
            catch (InvalidCastException e)
            {
                Debug.LogError($"Error al recibir datos: {e.Message}");
            }
        }
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
    public void SyncGameState(int syncedCoins, int syncedLives, int syncedLevel)
    {
        coins = syncedCoins;
        lives = syncedLives;
        level = syncedLevel;
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
        level = PlayerPrefs.GetInt("Level", 3); // Si no hay datos guardados, usa el nivel 1 como predeterminado
    }

    [PunRPC]
    public void GameOver()
    {
        lives = 3; // Reiniciar vidas
        // Guardar vidas, monedas y nivel en PlayerPrefs
        SaveGameData();

        Debug.Log($"GameOver llamado en el cliente: {PhotonNetwork.NickName}");
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.LoadLevel(2); // Sincronizar nivel en red
        }
        else
        {
            SceneManager.LoadScene(2); // Cargar la escena de victoria localmente
        }
    }

    [PunRPC]
    public void Victory()
    {
        ++level;
        if (level >= SceneManager.sceneCountInBuildSettings)
        {
            --level; // Reiniciar el nivel si es mayor o igual a 3
        }

        // Guardar vidas, monedas y nivel en PlayerPrefs
        SaveGameData();

        // Mostrar el panel de Victoria
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.LoadLevel(1); // Sincronizar nivel en red
        }
        else
        {
            SceneManager.LoadScene(1); // Cargar la escena de victoria localmente
        }
    }

    public void GameOverRPC()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            photonView.RPC("GameOver", RpcTarget.All); // Asegúrate de usar RpcTarget.All
        }
        else
        {
            GameOver(); // Llamar al método localmente si está en modo offline
        }
    }

    public void VictoryRPC()
    {
        if (!PhotonNetwork.OfflineMode)
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
        Debug.Log($"Jugador {newPlayer.NickName} se unió a la sala.");
        photonView.RPC("SyncGameState", newPlayer, coins, lives, level);
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


}
