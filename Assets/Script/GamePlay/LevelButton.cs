using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelButton : MonoBehaviour
{
    public int levelIndex; 
    public Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); 
        if (levelIndex > unlockedLevel) 
        { 
            button.interactable = false; 
        }
    }

    public void LoadLevel() 
    { 
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); 
        if (levelIndex <= unlockedLevel) 
        { 
            SceneManager.LoadScene(levelIndex); 
        } 
    }
}
