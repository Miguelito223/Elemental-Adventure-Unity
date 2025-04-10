using UnityEngine;
using Photon.Pun;


public class EnergyBall : MonoBehaviourPunCallbacks
{
    public AudioClip pickupSound; // Sonido al recoger la moneda
    private AudioSource audioSource;
    public string EnergyID; // Identificador único para la moneda

    void Start()
    {
        // Verificar si ya existe un GUID guardado para este enemigo
        if (PlayerPrefs.HasKey(gameObject.name + "_GUID"))
        {
            // Recuperar el GUID guardado
            EnergyID = PlayerPrefs.GetString(gameObject.name + "_GUID");
        }
        else
        {
            // Generar un nuevo GUID y guardarlo
            EnergyID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(gameObject.name + "_GUID", EnergyID);
            PlayerPrefs.Save();
        }

        audioSource = GetComponent<AudioSource>();
        // Verificar si la moneda ya ha sido recogida
        if (PlayerPrefs.GetInt(EnergyID, 0) == 1)
        {
            // Si la moneda ya fue recogida, desactivarla
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador la toca
        {
            // Marcar la moneda como recogida
            PlayerPrefs.SetInt(EnergyID, 1);
            PlayerPrefs.Save();

            GameManager.instance.AddCoin(); // Sincronizar monedas

            audioSource.PlayOneShot(pickupSound); // Reproducir sonido

            // Desactiva la moneda y la destruye después de que termine el sonido
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject); // Destruir después de 5 segundos
            }
        }
    }
}
