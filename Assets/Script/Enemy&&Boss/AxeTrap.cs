using UnityEngine;

public class AxeTrap : MonoBehaviour
{
    public int damage = 5;

    bool canDamage;

    PlayerMovement player;

    public void EnableDamage()
    {
        canDamage = true;

        if (player != null)
        {
            player.TakeDamage(damage);
            canDamage = false;
        }
    }

    public void DisableDamage()
    {
        canDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = col.GetComponent<PlayerMovement>();

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = null;
        }
    }
}