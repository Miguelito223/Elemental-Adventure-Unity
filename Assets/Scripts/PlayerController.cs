using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{

    bool canjump;
    public float speed = 50f;
    public float gravity = -9.80f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.GetComponent<CharacterController>().Move(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.GetComponent<CharacterController>().Move(new Vector3(speed * Time.deltaTime, 0, 0));

        }

        gameObject.GetComponent<CharacterController>().Move(new Vector3(0, gravity * Time.deltaTime, 0));

        managejump();
    }

    void managejump()
    {
        if (transform.position.y <= 0)
        {
            canjump = true;
        }


        if (Input.GetKey(KeyCode.Space) & canjump & transform.position.y <= 50) 
        {
            gameObject.GetComponent<CharacterController>().Move(new Vector3(0, speed * Time.deltaTime, 0));

            if (transform.position.y >= 10);
            {
                gameObject.GetComponent<CharacterController>().Move(new Vector3(0, speed * Time.deltaTime, 0));
            }
        }
    }
}
