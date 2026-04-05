using UnityEngine;
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

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isShowingDialogue = false;

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
    } public void ShowTimerDisplay(float timeRemaining)
    {
        // Munadir: Display battle timer in HP text slot
        // (reuse hpText for timer in Level 5)
        if (hpText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            hpText.text = "TIME: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}