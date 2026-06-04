using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pausePanel;
    public GameObject confirmMenuPanel;

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
        instance = this;
        
    }
    void Start()
    {
        
        Time.timeScale = 1f;
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

        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

        
        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt("Level_" + nextLevel, 1);

            PlayerPrefs.SetInt("CurrentLevel",nextLevel);
        }
        if (PlayerPrefs.GetInt("HiddenGateFound", 0) == 1)
        {
            PlayerPrefs.SetInt("HiddenGateUnlocked", 1);
            PlayerPrefs.DeleteKey("HiddenGateFound");
        }

        PlayerPrefs.Save();
        

        Time.timeScale = 0f;
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false);

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        PlayerPrefs.SetInt("TargetScene",currentScene);

        SceneManager.LoadScene("LoadingScene");
    }
    public void NextGame()
    {
        Time.timeScale = 1f;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene < 3)
        {
            
            PlayerPrefs.SetInt("TargetScene", nextScene);
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            Debug.Log("Hết game");
        }
    }
    public void NextDoor()
    {
        Time.timeScale = 1f;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene < 7)
        {
            PlayerPrefs.SetInt("TargetScene", nextScene);
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            Debug.Log("Hết game");
        }
    }
    public void MenuGame()
    {
        confirmMenuPanel.SetActive(true);
    }

    public void ConfirmMenu()
    {
        Time.timeScale = 1f;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false);
        confirmMenuPanel.SetActive(false);

        PlayerPrefs.SetInt("TargetScene", 0);

        SceneManager.LoadScene("LoadingScene");
    }
    public void CancelMenu()
    {
        confirmMenuPanel.SetActive(false);
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