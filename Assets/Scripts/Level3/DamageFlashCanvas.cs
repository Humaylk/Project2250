using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashCanvas : MonoBehaviour
{
    public static DamageFlashCanvas Instance { get; private set; }

    [Range(0f, 1f)] public float maxAlpha = 0.85f;
    public float fadeInDuration = 0.08f;
    public float fadeOutDuration = 0.5f;

    private Image flashImage;
    private Coroutine flashCoroutine;

    void Awake()
    {
        Instance = this;
        flashImage = GetComponentInChildren<Image>(true);
        if (flashImage != null)
            flashImage.color = new Color(1f, 1f, 1f, 0f);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Flash()
    {
        if (flashCoroutine != null) return;
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, maxAlpha, elapsed / fadeInDuration));
            yield return null;
        }
        SetAlpha(maxAlpha);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(maxAlpha, 0f, elapsed / fadeOutDuration));
            yield return null;
        }
        SetAlpha(0f);
        flashCoroutine = null;
    }

    private void SetAlpha(float a)
    {
        if (flashImage != null)
            flashImage.color = new Color(1f, 1f, 1f, a);
    }
}
