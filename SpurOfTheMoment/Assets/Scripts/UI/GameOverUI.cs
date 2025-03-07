using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI winnerText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverCanvas.SetActive(false); // Ensure it's hidden at start
    }

    public void ShowGameOver(string message)
    {
        gameOverCanvas.SetActive(true);
        winnerText.text = message;
        Time.timeScale = 0; // Pause the game
    }

    public void ReturnMainMenu()
    {
        MusicManager.Instance.PlayMusic(0);
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume game speed
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
