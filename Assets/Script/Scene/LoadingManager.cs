using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        Time.timeScale = 1f;

        int targetScene = PlayerPrefs.GetInt("TargetScene", 1);

        // Chờ Unity ổn định
        yield return null;

        yield return new WaitForSecondsRealtime(1.05f);

        SceneManager.LoadScene(targetScene);
    }
}