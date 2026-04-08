using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            // A new scene has its own AudioManager — swap to the new clip
            if (backgroundMusic != null && backgroundMusic != Instance.audioSource.clip)
            {
                Instance.audioSource.Stop();
                Instance.audioSource.clip = backgroundMusic;
                Instance.audioSource.volume = volume;
                Instance.audioSource.Play();
            }
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioSource.clip   = backgroundMusic;
        audioSource.volume = volume;
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        if (audioSource != null)
            audioSource.volume = volume;
    }
}
