using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer spriterender;
    public Animator animator;
    public Rigidbody2D RB2d;
    public AudioSource audiosource;

    bool canjump;
    bool isinground;
    float speed = 5000f;

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
        if (Input.GetKey(KeyCode.D))
        {
            RB2d.AddForce(new Vector2(speed * Time.deltaTime, 0));
            animator.SetBool("moving", true);
            spriterender.flipX = false;

            if (!audiosource.isPlaying && isinground)
            {
                audiosource.Play();
            }
            
        }
        if (Input.GetKey(KeyCode.A))
        {
            RB2d.AddForce(new Vector2(-speed * Time.deltaTime, 0 ));
            animator.SetBool("moving", true);
            spriterender.flipX = true;

            if (!audiosource.isPlaying && isinground)
            {
                audiosource.Play();
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetBool("moving", false);

            if (audiosource.isPlaying)
            {
                audiosource.Pause();
            }
        }

        if (audiosource.isPlaying && !isinground)
        {
            audiosource.Pause();
        }

        if (Input.GetKey(KeyCode.Space) && canjump)
        {
            canjump = false;
            animator.SetBool("jumping", true);
            RB2d.AddForce(new Vector2(0, speed));
        }

        if (gameObject.GetComponent<Rigidbody2D>().linearVelocity.y < 0)
        {
            animator.SetBool("jumping", false);
            animator.SetBool("falling", true);
        }
        else
        {
            animator.SetBool("falling", false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "terrain")
        {
            canjump = true;
            isinground = true;
            animator.SetBool("falling", false);
            animator.SetBool("jumping", false);
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
