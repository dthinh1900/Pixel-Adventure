using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public GameObject settingsPanel;
    public GameObject levelPanel;

    public void PlayGame()
    {
        
        Time.timeScale = 1f;

        PlayerPrefs.SetInt("TargetScene", 1);
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenSettings()
    {
        
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        
    }

    public void OpenLevelSelect()
    {
        
        levelPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelPanel.SetActive(false);
        
    }
    public void LoadLevel(int level)
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("TargetScene", level);

        SceneManager.LoadScene("LoadingScene");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
