using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField]
    private float m_timeToLive = 10f;

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();

        StartCoroutine(CountToDeath());
    }

    private IEnumerator CountToDeath()
    {
        yield return new WaitForSeconds(m_timeToLive);

        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
