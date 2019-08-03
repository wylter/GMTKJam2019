using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private int m_levelIndex;
    [SerializeField]
    private GameObject m_camera;
    [SerializeField]
    private Transform m_spawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.ToLevel(m_levelIndex);
        }

        GameController.instance.currentSpawnPosition = m_spawnPosition;
    }

    public void SetCameraActive(bool active)
    {
        m_camera.SetActive(active);
    }
}
