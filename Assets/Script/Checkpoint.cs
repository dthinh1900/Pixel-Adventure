using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Animator anim;
    private bool activated = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (activated) return;

        if (col.CompareTag("Player"))
        {
            activated = true;

            col.GetComponent<PlayerMovement>()?.SetCheckpoint(transform.position);

            anim.SetTrigger("Active");
        }
    }
}
