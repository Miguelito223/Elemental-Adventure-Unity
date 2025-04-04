using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public AudioClip pickupSound; // Sonido al recoger la moneda
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador la toca
        {
            GameManager.instance.AddCoin();

            audioSource.PlayOneShot(pickupSound); // Reproducir sonido
               
            // Desactiva la moneda y la destruye después de que termine el sonido
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, pickupSound.length);
        }
    }
}
