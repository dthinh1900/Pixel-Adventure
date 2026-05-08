using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;

    bool canDamage = true;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && canDamage)
        {
            col.GetComponent<PlayerMovement>()?.TakeDamage(damage);
            StartCoroutine(DamageCooldown());
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}