using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformShooter : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab = null;
    [SerializeField]
    private float m_spawnTime = 1f;

    private float m_lastTime;

    private void Start()
    {
        m_lastTime = Time.time;
        GameObject instance = Instantiate(m_prefab, transform.position, transform.rotation);
        instance.transform.parent = transform;
    }

    private void Update()
    {
        if (Time.time > m_lastTime + m_spawnTime)
        {
            m_lastTime = Time.time;
            GameObject instance = Instantiate(m_prefab, transform.position, transform.rotation);
            instance.transform.parent = transform;
        }
    }
}
