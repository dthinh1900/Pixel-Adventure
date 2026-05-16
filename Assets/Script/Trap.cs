using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;

    PlayerMovement player;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = col.GetComponent<PlayerMovement>();

            StartCoroutine(DamageLoop());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player = null;
        }
    }

    IEnumerator DamageLoop()
    {
        while (player != null)
        {
            player.TakeDamage(damage);

            yield return new WaitForSeconds(damageCooldown);
        }
    }
}