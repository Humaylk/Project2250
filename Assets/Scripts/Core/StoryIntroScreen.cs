using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryIntroScreen : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text storyText;
    public TMP_Text promptText;

    public float lineDuration = 2.5f;
    public string nextSceneName = "Level1";

    private string[] storyLines = {
        "Long ago, five elemental gods maintained balance across the realm...",
        "Quaziadore kept peace between Earth, Fire, Water, Air, and Aether.",
        "But humanity began harnessing elemental magic for its own gain.",
        "The gods turned against each other.",
        "The world shattered into five floating islands,",
        "each corrupted by a different element.",
        "You are Alex. Chosen by fate.",
        "Reclaim the elemental cores. Free the islands. End the war."
    };

    void Start()
    {
        if (promptText != null) promptText.alpha = 0f;
        if (storyText != null) storyText.text = "";
        StartCoroutine(PlayStory());
    }

    private IEnumerator PlayStory()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (string line in storyLines)
        {
            if (storyText != null)
                storyText.text = line;
            yield return new WaitForSeconds(lineDuration);
        }
        StartCoroutine(FadeInPrompt());
    }

    private IEnumerator FadeInPrompt()
    {
        if (promptText == null) yield break;
        promptText.text = "Press any key to begin...";
        while (promptText.alpha < 1f)
        {
            promptText.alpha += Time.deltaTime;
            yield return null;
        }
        while (!Input.anyKeyDown)
            yield return null;
        SceneManager.LoadScene(nextSceneName);
    }
}