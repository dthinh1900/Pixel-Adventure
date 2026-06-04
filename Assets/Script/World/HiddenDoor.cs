using UnityEngine;

public class HiddenDoor : MonoBehaviour
{
    public GameObject popupUI;

    bool triggered = false;

    void Start()
    {
        popupUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (triggered) return;

        if (col.CompareTag("Player"))
        {
            triggered = true;

            popupUI.SetActive(true);

            PlayerPrefs.SetInt("HiddenGateFound", 1);

            Debug.Log("Hidden Gate Unlocked!");
        }
    }
    public void ClosePopup()
    {
        popupUI.SetActive(false);
    }
}