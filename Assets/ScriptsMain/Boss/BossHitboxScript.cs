using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitboxScript : MonoBehaviour
{
    PlayerMovement playerScript;

    private void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript.Knock(6);
        }
    }
}
