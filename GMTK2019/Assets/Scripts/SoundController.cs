using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private AudioMixer m_audioMixer = null;                  //Reference to the audio mixer of the project
    [SerializeField]
    private AudioSource m_efxSource = null;                   //Drag a reference to the audio source which will play the sound effects.
    [SerializeField]
    private AudioSource m_musicSource = null;                 //Drag a reference to the audio source which will play the music.

    public static SoundController instance = null;     //Allows other scripts to call functions from SoundManager.             
    [SerializeField]
    private float m_lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    [SerializeField]
    private float m_highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

    [Space]
    [Header("Song")]
    [SerializeField]
    private AudioClip m_firstSongClip;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeSong(m_firstSongClip);
    }

    public void SetVolume(float volume)
    {
        m_audioMixer.SetFloat("Volume", volume);
    }

    public float GetVolume()
    {
        float volume = 0;
        m_audioMixer.GetFloat("Volume", out volume);
        return volume;
    }


    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, float volumeMultiplier = 1f)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        m_efxSource.clip = clip;

        //Set the pitch of the audio source to the randomly chosen pitch.
        m_efxSource.pitch = Random.Range(m_lowPitchRange, m_highPitchRange) * volumeMultiplier;

        //Play the clip.
        m_efxSource.Play();
    }

    //Changes the songs between levels (Only if the next level has a different music)
    public void ChangeSong(AudioClip clip)
    {
        if (m_musicSource.clip != clip)
        {
            m_musicSource.clip = clip;

            //Play the clip.
            m_musicSource.Play();
        }
    }
}
