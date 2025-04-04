using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriterender;
    public Animator animator;
    public Rigidbody2D RB2d;
    public AudioSource audiosource;
    public Transform bulletPos;
    public Transform bulletSpawn;
    public GameObject bullet;

    bool canjump;
    bool isinground;
    public float speed = 1000f;
    public float jumpForce = 1000f;
    public float bulletCooldown = 1f; // Tiempo de espera entre disparos (1 segundo)
    public float bulletSpeed = 10f;
    bool canShoot = true;

    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        RB2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterender = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!photonView.IsMine && !PhotonNetwork.OfflineMode) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        RB2d.linearVelocity = new Vector2(moveInput * speed * Time.deltaTime, RB2d.linearVelocity.y);

        animator.SetBool("moving", moveInput != 0);
        spriterender.flipX = moveInput < 0;

        if (!audiosource.isPlaying && isinground && moveInput != 0) audiosource.Play();
        else if ((moveInput == 0 || !isinground) && audiosource.isPlaying) audiosource.Pause();

        if (Input.GetKeyDown(KeyCode.Space) && canjump)
        {
            canjump = false;
            isinground = false;
            animator.SetBool("jumping", true);
            RB2d.linearVelocity = new Vector2(RB2d.linearVelocity.x, jumpForce * Time.deltaTime);
        }

        // Calcular la dirección hacia el mouse (esto ya no afecta al jugador)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Asegurarse de que esté en el plano 2D (sin componente z)
        Vector2 direction = (mousePosition - bulletPos.position).normalized;

        // Rotar solo la bala cuando se dispare, no el jugador
        if (Input.GetKeyDown(KeyCode.S) && canShoot)
        {
            StartCoroutine(Shoot(direction)); // Pasa la dirección hacia el método de disparo
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

    private IEnumerator Shoot(Vector2 direction)
    {
        canShoot = false; // Bloquea disparos hasta que termine el cooldown

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletPos.transform.rotation = Quaternion.Euler(0, 0, angle); // Girar la bala

        // Instanciar la bala en la posición de disparo
        GameObject bulletInstance = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();

        // Aplicar la velocidad a la bala
        bulletRb.linearVelocity = direction * bulletSpeed;

        yield return new WaitForSeconds(bulletCooldown); // Espera el tiempo del cooldown

        canShoot = true; // Permite disparar nuevamente
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
}
