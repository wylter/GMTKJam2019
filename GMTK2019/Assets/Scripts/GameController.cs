using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public static GameController instance;
    [SerializeField]
    private PlayerController m_player;
    [SerializeField]
    private List<Level> m_levels;

    public Transform currentSpawnPosition;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            ResetToLastSpawn();
        }
    }

    private void ResetToLastSpawn()
    {
        m_player.transform.position = currentSpawnPosition.position;
        m_player.m_rb.velocity = Vector2.zero;
        m_player.RecoverJump();
    }

    private void Init()
    {
        for (int i = 1; i < m_levels.Count; i++)
        {
            m_levels[i].SetCameraActive(false);
        }
    }

    public void ToLevel(int levelIndex)
    {
        for (int i = 0; i < m_levels.Count; i++)
        {
            m_levels[i].SetCameraActive(false);
        }

        m_levels[levelIndex].SetCameraActive(true);

        m_player.RecoverJump();
    }
}
