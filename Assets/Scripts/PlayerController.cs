using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriterender;
    public Animator animator;
    public Rigidbody2D RB2d;
    public AudioSource audiosource;
    public GameObject bullet;

    bool canjump;
    bool isinground;
    float speed = 5000f;
    float jumpForce = 2000f;
    float BulletTime;
    bool CanBullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audiosource = gameObject.GetComponent<AudioSource>();
        RB2d = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spriterender = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine & !PhotonNetwork.OfflineMode) 
        {
            return;
        }

        float moveInput = Input.GetAxisRaw("Horizontal"); // Movimiento más preciso

        // Mover personaje
        RB2d.linearVelocity = new Vector2(moveInput * speed * Time.deltaTime, RB2d.linearVelocity.y);

        // Control de animaciones
        if (moveInput != 0)
        {
            animator.SetBool("moving", true);
            spriterender.flipX = moveInput < 0; // Voltear el sprite dependiendo de la dirección
        }
        else
        {
            animator.SetBool("moving", false);
        }

        // Control del sonido al caminar
        if (!audiosource.isPlaying && isinground && moveInput != 0)
        {
            audiosource.Play();
        }
        else if ((moveInput == 0 || !isinground) && audiosource.isPlaying)
        {
            audiosource.Pause();
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && canjump)
        {
            canjump = false;
            isinground = false;
            animator.SetBool("jumping", true);
            RB2d.linearVelocity = new Vector2(RB2d.linearVelocity.x, jumpForce * Time.deltaTime); // Usar velocity en vez de AddForce
        }

        if (Input.GetKeyDown(KeyCode.S) && CanBullet)
        {
            GameObject bulletIst = Instantiate(bullet, transform.position, transform.rotation)
            bulletIst.RGB2d.
        }

        // Detectar caída
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
        if (collision.transform.tag == "terrain")
        {
            isinground = false;
        }
    }
}
