using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    private void Start()
    {
        GameController.instance.m_respawnList.Add(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.player.RecoverJump();
            gameObject.SetActive(false);
        }
    }
}
