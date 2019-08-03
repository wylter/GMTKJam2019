using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public static GameController instance;
    [SerializeField]
    private PlayerController m_player = null;
    [SerializeField]
    private List<Level> m_levels = null;
    [HideInInspector]
    public Transform currentSpawnPosition;
    [HideInInspector]
    public int overlapNum = 0;
    [HideInInspector]
    public List<GameObject> m_respawnList;

    public PlayerController player
    {
        get { return m_player; }
    }

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

        m_respawnList = new List<GameObject>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Init();

        EnforceResolution();
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

        for (int i = 0; i < m_respawnList.Count; i++)
        {
            m_respawnList[i].SetActive(true);
        }
    }

    private void Init()
    {
        for (int i = 1; i < m_levels.Count; i++)
        {
            m_levels[i].SetCameraActive(false);
        }

        //m_levels[0].SetAsCurrentLevel();
        //m_levels[0].m_insideMe = true;
    }

    public void ToLevel(int levelIndex)
    {
        for (int i = 0; i < m_levels.Count; i++)
        {
            m_levels[i].SetCameraActive(false);
            m_levels[i].m_insideMe = false;
        }

        m_levels[levelIndex].SetCameraActive(true);
        m_levels[levelIndex].m_insideMe = true;

        m_player.RecoverJump();
    }

    public void KillPlayer()
    {
        ResetToLastSpawn();
    }

    private void EnforceResolution()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
