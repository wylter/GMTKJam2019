using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private int m_levelIndex;
    [SerializeField]
    private GameObject m_camera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.instance.ToLevel(m_levelIndex);
        }
            
    }

    public void SetCameraActive(bool active)
    {
        m_camera.SetActive(active);
    }
}
