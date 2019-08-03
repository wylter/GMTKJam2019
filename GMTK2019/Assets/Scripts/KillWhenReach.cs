using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillWhenReach : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform"))
        {
            Destroy(collision.gameObject);
        }
    }
}
