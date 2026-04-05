using System.Collections;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public float frameDelay = 0.08f;

    [Header("Animation Frames (frame 00 to 04)")]
    public Sprite[] openingFrames;

    [Header("Audio")]
    public AudioClip proximitySound;
    [Range(0f, 1f)] public float maxVolume = 1f;
    public float fadeDuration = 1.5f;

    [Header("Reward")]
    public GameObject helmetObject;
    public float helmetFadeDuration = 2f;


    private SpriteRenderer sr;
    private AudioSource audioSource;
    private Transform player;
    private bool isOpen = false;
    private bool soundStarted = false;
    private bool isFading = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.loop = true;
        audioSource.volume = 0f;

        int frameCount = openingFrames != null ? openingFrames.Length : 0;
        Debug.Log("ChestInteraction: Started. Frames loaded = " + frameCount);

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            player = pc.transform;
            Debug.Log("ChestInteraction: Player found.");
        }
        else
            Debug.LogWarning("ChestInteraction: PlayerController NOT found!");
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool inRange = dist <= interactRange;

        // Start looping sound when player enters range
        if (inRange && !soundStarted && !isOpen && proximitySound != null)
        {
            soundStarted = true;
            isFading = false;
            audioSource.clip = proximitySound;
            audioSource.volume = 0f;
            audioSource.Play();
            StartCoroutine(FadeIn());
        }
        // Fade out when player walks away (and chest isn't already handling the fade)
        else if (!inRange && soundStarted && !isFading)
        {
            StartCoroutine(FadeOut(resetOnComplete: true));
        }

        if (isOpen) return;

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("ChestInteraction: E pressed, dist=" + dist + ", starting animation.");
            isOpen = true;
            if (soundStarted && !isFading)
                StartCoroutine(FadeOut(resetOnComplete: false));
            StartCoroutine(PlayOpenAnimation());
            if (helmetObject != null)
                StartCoroutine(FadeInHelmet());
        }
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            // Stop fading in if we need to fade out (e.g. player walked away immediately)
            if (isFading) yield break;
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = maxVolume;
    }

    IEnumerator FadeOut(bool resetOnComplete)
    {
        isFading = true;
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        isFading = false;

        // If player just walked away (not opened), allow sound to restart if they return
        if (resetOnComplete)
            soundStarted = false;
    }

    IEnumerator FadeInHelmet()
    {
        SpriteRenderer helmetSr = helmetObject.GetComponent<SpriteRenderer>();
        if (helmetSr == null) yield break;

        // Wait for the chest opening animation to finish first
        float animationDuration = openingFrames.Length * frameDelay;
        yield return new WaitForSeconds(animationDuration);

        float elapsed = 0f;
        Color c = helmetSr.color;
        c.a = 0f;
        helmetSr.color = c;

        while (elapsed < helmetFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / helmetFadeDuration);
            helmetSr.color = c;
            yield return null;
        }

        c.a = 1f;
        helmetSr.color = c;
    }

    IEnumerator PlayOpenAnimation()
    {
        Debug.Log("ChestInteraction: Playing open animation, frames=" + openingFrames.Length);

        for (int i = 0; i < openingFrames.Length; i++)
        {
            if (openingFrames[i] == null)
            {
                Debug.LogWarning("ChestInteraction: Frame " + i + " is NULL!");
                continue;
            }
            sr.sprite = openingFrames[i];
            Debug.Log("ChestInteraction: Set frame " + i);
            yield return new WaitForSeconds(frameDelay);
        }

        GameManager.Instance?.uiManager?.ShowHint("Chest opened!");
        Debug.Log("ChestInteraction: Done.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
