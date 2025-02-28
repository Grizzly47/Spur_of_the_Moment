using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        MusicManager.Instance.PlayMusic(1);
        SceneManager.LoadScene("Gameplay"); // Replace "GameplayScene" with your actual scene name
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
