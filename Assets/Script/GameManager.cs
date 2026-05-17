using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pausePanel;

    [Header("Win UI")]
    public TMP_Text Win_soulCollectedText;
    public TMP_Text Win_starCollectedText;
    public TMP_Text Win_timeText;

    [Header("OG UI")]
    public TMP_Text OG_soulCollectedText;
    public TMP_Text OG_starCollectedText;
    public TMP_Text OG_timeText;
    PlayerMovement player;

    bool isPaused = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }
    void Update()
    {
        if (isPaused) return;
    }
    public void GameOver()
    {
        OG_soulCollectedText.text = "X" + player.GetSoul();
        OG_starCollectedText.text = "X" + player.GetStar();
        OG_timeText.text = player.GetFormattedTime();

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        Win_soulCollectedText.text = "X" + player.GetSoul();
        Win_starCollectedText.text = "X" + player.GetStar();
        Win_timeText.text = player.GetFormattedTime();

        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextGame()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("Hết game");
        }
    }
    public void MenuGame()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false);

        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
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