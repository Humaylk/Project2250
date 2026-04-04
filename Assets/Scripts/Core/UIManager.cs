using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Text Elements")]
    public TMP_Text objectiveText;
    public TMP_Text hintText;
    public TMP_Text hpText;
    public TMP_Text dialogueText;

    public float hintDisplayDuration = 4f;
    public float dialogueDisplayDuration = 3f;

    [Header("Damage Flash")]
    public Sprite damageSprite;

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isShowingDialogue = false;
    private Image damageFlashImage;

    void Start()
    {
        // Initial HP display
        if (hpText != null)
            hpText.text = "HP: 100";

        // FORCE dialogue style (this fixes your issue)
        if (dialogueText != null)
        {
            dialogueText.enableAutoSizing = false;
            dialogueText.fontSize = 36;
            dialogueText.color = Color.white;
            dialogueText.fontStyle = FontStyles.Bold;
        }
    }

    public void DisplayObjective(string objective)
    {
        if (objectiveText != null)
            objectiveText.text = "OBJECTIVE: " + objective;
    }

    public void UpdateObjective(string newObjective) => DisplayObjective(newObjective);

    public void ShowHint(string hint)
    {
        if (hintText == null) return;

        hintText.text = hint;
        StartCoroutine(ClearHint(hintDisplayDuration));
    }

    public void UpdateHPDisplay(int hp)
    {
        if (hpText != null)
            hpText.text = "HP: " + hp;
    }

    // 🔥 UPDATED — forces bold + color using rich text
    public void QueueDialogue(string message)
    {
        // Force bold + white color every time
        string styledMessage = $"<b><color=white>{message}</color></b>";

        dialogueQueue.Enqueue(styledMessage);

        if (!isShowingDialogue)
            StartCoroutine(ProcessDialogueQueue());
    }

    public void ClearMessages()
    {
        if (hintText != null) hintText.text = "";
        if (dialogueText != null) dialogueText.text = "";

        dialogueQueue.Clear();
        isShowingDialogue = false;
    }

    public void FlashDamage()
    {
        if (damageFlashImage == null)
        {
            // Dedicated canvas so it always renders on top of everything
            GameObject canvasGO = new GameObject("DamageFlashCanvas");
            DontDestroyOnLoad(canvasGO);
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasGO.AddComponent<CanvasScaler>();

            GameObject imageGO = new GameObject("DamageFlashImage");
            imageGO.transform.SetParent(canvasGO.transform, false);
            damageFlashImage = imageGO.AddComponent<Image>();
            damageFlashImage.raycastTarget = false;
            damageFlashImage.color = new Color(1f, 1f, 1f, 0f);

            if (damageSprite != null)
            {
                damageFlashImage.sprite = damageSprite;
                damageFlashImage.type = Image.Type.Simple;
                damageFlashImage.preserveAspect = false;
            }

            RectTransform rt = imageGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        StopCoroutine("DamageFlashCoroutine");
        StartCoroutine("DamageFlashCoroutine");
    }

    private IEnumerator DamageFlashCoroutine()
    {
        damageFlashImage.color = new Color(1f, 1f, 1f, 0.85f);
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            damageFlashImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.85f, 0f, elapsed / duration));
            yield return null;
        }
        damageFlashImage.color = new Color(1f, 1f, 1f, 0f);
    }

    private IEnumerator ClearHint(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hintText != null) hintText.text = "";
    }

    private IEnumerator ProcessDialogueQueue()
    {
        isShowingDialogue = true;

        while (dialogueQueue.Count > 0)
        {
            string msg = dialogueQueue.Dequeue();

            if (dialogueText != null)
                dialogueText.text = msg;

            yield return new WaitForSeconds(dialogueDisplayDuration);
        }

        if (dialogueText != null)
            dialogueText.text = "";

        isShowingDialogue = false;
    }
}