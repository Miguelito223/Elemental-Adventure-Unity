using Photon.Pun;
using System.Collections;
using UnityEngine;

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
    public GameObject gameovermenu;
    public GameObject victorymenu;

    bool canjump;
    bool isinground;
    public float speed = 100f; // Ajusta la velocidad según sea necesario
    public float bulletCooldown = 1f; // Tiempo de espera entre disparos (1 segundo)
    public float bulletSpeed = 10f;
    bool canShoot = true;

    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine)
            {
                playercamera.enabled = false;
                coinspanel.SetActive(false);
                lifespanel.SetActive(false);
                gameovermenu.SetActive(false);
                victorymenu.SetActive(false);
                return;
            }
        }

        audiosource = GetComponent<AudioSource>();
        RB2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();

        playercamera.enabled = true; // Habilitar la cámara del jugador
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
                gameovermenu.SetActive(false);
                victorymenu.SetActive(false);
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
        if (collision.transform.CompareTag("terrain"))
        {
            isinground = true;
            canjump = true;
            animator.SetBool("jumping", false);
            animator.SetBool("falling", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("terrain"))
        {
            isinground = false;
        }
    }

    // Sincronizar datos con Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("Enviando");
            // Enviar el estado de flipX a otros jugadores
            stream.SendNext(spriterender.flipX);
            stream.SendNext(canjump);
            stream.SendNext(isinground);
            stream.SendNext(bulletSpeed);
            stream.SendNext(canShoot);
            stream.SendNext(bulletCooldown);
            stream.SendNext(speed);
            stream.SendNext(audiosource.isPlaying);
        }
        else
        {
            Debug.Log("Recibiendo");
            spriterender.flipX = (bool)stream.ReceiveNext();
            canjump = (bool)stream.ReceiveNext();
            isinground = (bool)stream.ReceiveNext();
            bulletSpeed = (int)stream.ReceiveNext();
            canShoot = (bool)stream.ReceiveNext();
            bulletCooldown = (int)stream.ReceiveNext();
            speed = (int)stream.ReceiveNext();
        }
    }

}
