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

    public bool m_insideMe = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.overlapNum++;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !m_insideMe && GameController.instance.overlapNum == 1)
        {
            SetAsCurrentLevel();
        }
    }

    public void SetAsCurrentLevel()
    {
        GameController.instance.ToLevel(m_levelIndex);

        GameController.instance.currentSpawnPosition = m_spawnPosition;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.overlapNum--;
        }
    }

    public void SetCameraActive(bool active)
    {
        m_camera.SetActive(active);
    }
}
