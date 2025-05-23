using UnityEngine;
using Photon.Pun;


public class EnergyBall : MonoBehaviourPunCallbacks, IPunObservable
{
    public AudioClip EnergySound; // Sonido al recoger la moneda  
    public AudioSource audioSource;
    public string EnergyID; // Identificador �nico para la moneda  

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

        // Verificar si la moneda ya ha sido recogida  
        if (PlayerPrefs.GetInt(EnergyID, 0) == 1)
        {
            // Si la moneda ya fue recogida, destruirla 
            if (!PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject); // Destruir despu�s de 5 segundos  
            }
        }
    }

    public void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Verifica si el jugador la toca  
        {
            // Marcar la moneda como recogida  
            PlayerPrefs.SetInt(EnergyID, 1);
            PlayerPrefs.Save();

            collision.gameObject.GetComponent<PlayerController>().AddCoin(); // Sincronizar monedas  
            
            AudioClip Sound = EnergySound; // Sonido al recoger la moneda
            audioSource.PlayOneShot(Sound); // Reproducir sonido  

            // Desactiva la moneda y la destruye despu�s de que termine el sonido  
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            if (!PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject); // Destruir despu�s de 5 segundos  
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar datos al otro jugador  
            stream.SendNext(EnergyID);
            // Enviar el estado de la moneda (activa o no) al resto de los jugadores
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            EnergyID = (string)stream.ReceiveNext();
            // Recibir el estado de la moneda desde otro jugador
            bool isActive = (bool)stream.ReceiveNext();
            gameObject.SetActive(isActive);
        }
    } 
}
