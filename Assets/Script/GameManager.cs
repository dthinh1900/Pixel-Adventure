using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pausePanel;

    bool isPaused = false;
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (isPaused) return;
    }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PauseGame()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

}