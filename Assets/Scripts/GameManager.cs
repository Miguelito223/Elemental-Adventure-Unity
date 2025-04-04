using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Patrón Singleton para acceder desde otros scripts
    public GameObject gameOverPanel; // Arrastra el panel de Game Over aquí

    public int coins = 0;
    public int lives = 3;
    

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    public void AddCoin()
    {
        coins++;
        UpdateUI();
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Mostrar el panel de Game Over
        gameOverPanel.SetActive(true);
        // Detener el juego o hacer que el jugador no pueda moverse, por ejemplo:
        Time.timeScale = 0f; // Detiene el tiempo
    }

    public void Retry()
    {
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Asegúrate de que el tiempo vuelva a la normalidad
    }

    void UpdateUI()
    {
        coinText.text = "Monedas: " + coins;
        livesText.text = "Vidas: " + lives;
    }
}
