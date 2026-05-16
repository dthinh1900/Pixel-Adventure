using UnityEngine;

public class HeartItem : MonoBehaviour
{
    public int healAmount = 3;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerMovement>()?.Heal(healAmount);
            SoundManager.instance.PlaySound(SoundManager.instance.collectSFX);
            Destroy(gameObject);
        }
    }
}
