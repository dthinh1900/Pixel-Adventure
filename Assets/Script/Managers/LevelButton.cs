using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public Button button;

    [Header("Lock UI")]
    public GameObject lockObject;
    [Header("Unlock Cost")]
    public int starCost = 5;

    private bool isUnlocked;

    void Start()
    {
        CheckLevel();
    }

    void CheckLevel()
    {
        isUnlocked =
            PlayerPrefs.GetInt("Level_" + levelIndex, 0) == 1;

        lockObject.SetActive(!isUnlocked);

        button.interactable = true;
    }

    public void LoadLevel()
    {
        int star = PlayerPrefs.GetInt("Star", 0);

        if (isUnlocked)
        {
            PlayerPrefs.SetInt("TargetScene", levelIndex);

            SceneManager.LoadScene("LoadingScene");

            return;
        }

        if (star >= starCost)
        {
            star -= starCost;

            PlayerPrefs.SetInt("Star", star);

            PlayerPrefs.SetInt("Level_" + levelIndex, 1);
            PlayerPrefs.SetInt("TargetScene", levelIndex);
            PlayerPrefs.Save();                    
            SceneManager.LoadScene("LoadingScene");

            CheckLevel();

            Debug.Log("Đã mở khóa Level " + levelIndex);
        }
        else
        {
            Debug.Log("Không đủ Star");
        }
    }
}