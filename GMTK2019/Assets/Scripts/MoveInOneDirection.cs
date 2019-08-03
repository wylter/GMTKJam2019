using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInOneDirection : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_speed = Vector2.zero;

    private Rigidbody2D m_rb;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.velocity = m_speed;
    }

    private void Update()
    {
        //Vector2 position = new Vector2(transform.position.x, transform.position.y);
        //m_rb.MovePosition(position + m_speed * Time.deltaTime);
        //transform.position = position + m_speed * Time.deltaTime;
    }

    //     void OnCollisionEnter2D(Collision2D other)
    //     {
    //         if (other.transform.tag == "Player")
    //         {
    //             other.transform.parent = transform;
    //         }
    //     }
    // 
    //     private void OnCollisionExit2D(Collision2D other)
    //     {
    //         if (other.transform.tag == "Player")
    //         {
    //             other.transform.parent = null;
    //         }
    //     }
}
