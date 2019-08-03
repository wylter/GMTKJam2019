using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public static GameController instance;
    [SerializeField]
    private List<Level> m_levels;

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
            SceneManager.LoadScene(0);
        }
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
    }
}
