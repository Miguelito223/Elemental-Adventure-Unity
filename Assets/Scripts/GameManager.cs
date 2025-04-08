using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Patrón Singleton para acceder desde otros scripts
    public GameObject gameOverPanel; // Arrastra el panel de Game Over aquí
    public GameObject victoryPanel; // Arrastra el panel de Victoria aquí

    public int coins = 0;
    public int lives = 3;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    private bool isInvulnerable = false; // Controla si el jugador está en estado de invulnerabilidad
    public float invulnerabilityDuration = 2f; // Duración del estado de invulnerabilidad en segundos

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
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
        if (isInvulnerable) return; // Si el jugador es invulnerable, no pierde vida

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(InvulnerabilityTimer()); // Inicia el temporizador de invulnerabilidad
        }
    }

    private IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true; // Activa el estado de invulnerabilidad
        yield return new WaitForSeconds(invulnerabilityDuration); // Espera el tiempo especificado
        isInvulnerable = false; // Desactiva el estado de invulnerabilidad
    }

    void GameOver()
    {
        // Mostrar el panel de Game Over
        gameOverPanel.SetActive(true);
        // Detener el juego o hacer que el jugador no pueda moverse, por ejemplo:
        Time.timeScale = 0f; // Detiene el tiempo
    }

    public void Victory()
    {
        // Mostrar el panel de Victoria
        victoryPanel.SetActive(true);
        // Detener el tiempo del juego
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    public void LoadMainMenu()
    {
        // Cargar el menú principal
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    void UpdateUI()
    {
        coinText.text = "Monedas: " + coins;
        livesText.text = "Vidas: " + lives;
    }
}

public class Box : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Verifica si el jugador toca el objeto
        {
            GameManager.instance.Victory(); // Llama al método de victoria
        }
    }
}
