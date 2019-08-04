using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{

    [SerializeField]
    private Animator m_animator = null;
//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             GameController.instance.player.Bounce();
//         }
//     }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.player.Bounce();
            m_animator.SetTrigger("Bounce");
        }
    }
}
