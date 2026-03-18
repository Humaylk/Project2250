using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// <summary>
// Manages all the game's user interface text elements, including objectives, hints, HP display, and dialogue.
// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Text Elements")]
    // References to the TextMeshPro text components in the scene canvas.
    public TMP_Text objectiveText;   // The main text area for displaying the current objective.
    public TMP_Text hintText;        // The text area for displaying temporary gameplay hints.
    public TMP_Text hpText;          // The text area for displaying the player's current health (HP).
    public TMP_Text dialogueText;    // The text area for displaying dialogue lines.

    // Durations (in seconds) for how long hints and dialogue lines should be displayed.
    public float hintDisplayDuration = 4f;
    public float dialogueDisplayDuration = 3f;

    // --- Private Fields for Message Management ---

    // A queue to store dialogue messages that need to be displayed sequentially.
    private Queue<string> dialogueQueue = new Queue<string>();
    // Flag to indicate if a dialogue coroutine is currently processing the queue.
    private bool isShowingDialogue = false;

    // --- Public UI Update Methods ---

    // <summary>
    // Updates the objective text component with a new objective message.
    // </summary>
    // <param name="objective">The new objective text to display.</param>
    public void DisplayObjective(string objective)
    {
        // Check if the text component reference is set before attempting to access it.
        if (objectiveText != null)
            objectiveText.text = "OBJECTIVE: " + objective;
    }

    // <summary>
    // A helper method that calls DisplayObjective to update the objective text.
    // </summary>
    // <param name="newObjective">The new objective text to display.</param>
    public void UpdateObjective(string newObjective) => DisplayObjective(newObjective);

    // <summary>
    // Displays a temporary hint message and automatically clears it after a set duration.
    // </summary>
    // <param name="hint">The hint text to show.</param>
    public void ShowHint(string hint)
    {
        // If the component is not set, exit.
        if (hintText == null) return;
        // Display the text.
        hintText.text = hint;
        // Start a coroutine to clear the hint after the specified delay.
        StartCoroutine(ClearHint(hintDisplayDuration));
    }

    // <summary>
    // Updates the player's health (HP) display text component.
    // </summary>
    // <param name="hp">The current HP value to display.</param>
    public void UpdateHPDisplay(int hp)
    {
        if (hpText != null)
            hpText.text = "HP: " + hp;
    }

    // <summary>
    // Adds a new dialogue message to the queue to be displayed. If no dialogue is currently showing, it starts processing the queue.
    // </summary>
    // <param name="message">The dialogue line to add to the queue.</param>
    public void QueueDialogue(string message)
    {
        // Enqueue the new message.
        dialogueQueue.Enqueue(message);
        // If the coroutine isn't already running, start it.
        if (!isShowingDialogue)
            StartCoroutine(ProcessDialogueQueue());
    }

    // <summary>
    // Instantly clears all active text from the hint and dialogue areas, and empties the dialogue queue.
    // </summary>
    public void ClearMessages()
    {
        // Clear active text if references exist.
        if (hintText != null) hintText.text = "";
        if (dialogueText != null) dialogueText.text = "";
        // Clear the queue and reset the coroutine flag.
        dialogueQueue.Clear();
        isShowingDialogue = false;
    }

    // --- Private Coroutines for Message Timing ---

    // <summary>
    // Waits for a specified delay and then clears the hint text component.
    // </summary>
    // <param name="delay">The time (in seconds) to wait before clearing the text.</param>
    private IEnumerator ClearHint(float delay)
    {
        // Wait for the duration.
        yield return new WaitForSeconds(delay);
        // Clear the hint text.
        if (hintText != null) hintText.text = "";
    }

    // <summary>
    // Iterates through the dialogue queue, displaying each message with a set duration and delay between lines.
    // </summary>
    private IEnumerator ProcessDialogueQueue()
    {
        // Set flag to indicate the coroutine is active.
        isShowingDialogue = true;
        // Continue processing as long as there are messages in the queue.
        while (dialogueQueue.Count > 0)
        {
            // Dequeue the next message to display.
            string msg = dialogueQueue.Dequeue();
            // Display the dialogue text if the reference is valid.
            if (dialogueText != null)
                dialogueText.text = msg;
            // Wait for the dialogue line duration.
            yield return new WaitForSeconds(dialogueDisplayDuration);
        }
        // After the queue is empty, clear the text area.
        if (dialogueText != null)
            dialogueText.text = "";
        // Reset flag to indicate the coroutine is finished.
        isShowingDialogue = false;
    }
}