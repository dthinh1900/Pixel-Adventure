using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public GameObject settingsPanel;
    public GameObject levelPanel;
    public GameObject secretDoorUI;
    public GameObject DoorPanel;
    public GameObject confirmQuitPanel;
    [Header("Door UI")]
    public TextMeshProUGUI starText;
    void Start()
    {
        
        if (!PlayerPrefs.HasKey("Level_1"))
        {
            PlayerPrefs.SetInt("Level_1", 1);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }

        PlayerPrefs.Save();

        secretDoorUI.SetActive(
            PlayerPrefs.GetInt("HiddenGateUnlocked", 0) == 1
        );


    }
    

    public void PlayGame()
    {

        Time.timeScale = 1f;

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        PlayerPrefs.SetInt("TargetScene", currentLevel);

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
    public void OpenDoorSelect()
    {

        DoorPanel.SetActive(true);
        int star = PlayerPrefs.GetInt("Star", 0);
        starText.text = "X " + star;
    }
    public void CloseDoorSelect()
    {
        DoorPanel.SetActive(false);

    }
    public void LoadLevel(int level)
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("TargetScene", level);

        SceneManager.LoadScene("LoadingScene");
    }
    public void QuitGame()
    {
        confirmQuitPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CancelQuit()
    {
        confirmQuitPanel.SetActive(false);
    }

}
