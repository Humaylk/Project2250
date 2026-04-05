using UnityEngine;
using TMPro;

public class DragonInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject      dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject      pressEPrompt;

    [HideInInspector] public bool hasAllItems = false;

    private string questDialogue =
        "Brave traveller... my power has been stripped from me and the bridge lies broken. " +
        "Seek out my Ancient Fire, and my Ancient Scroll, as for the bridge repairs seek some Wood, Stone, and Iron. " +
        "Only then can I restore the bridge and clear your path. Go!";

    private string completeDialogue =
        "You have done it! I can feel my power returning. " +
        "The bridge is restored — cross it and go forth, brave one!";

    private string alreadyDoneDialogue =
        "The bridge is open. Go, brave one. Your destiny awaits beyond.";

    private bool panelOpen   = false;
    private bool questDone   = false;
    private bool playerNearby = false;

    private void Start()
    {
        SetPanel(false);
        SetPrompt(false);
    }

    private void Update()
    {
        SetPrompt(playerNearby && !panelOpen);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerNearby && !panelOpen)
                OpenDialogue();
            else if (panelOpen)
                CloseDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (panelOpen) CloseDialogue();
        }
    }

    private void OpenDialogue()
    {
        panelOpen = true;
        SetPanel(true);
        SetPrompt(false);

        if (questDone)
            ShowText(alreadyDoneDialogue);
        else if (hasAllItems)
        {
            ShowText(completeDialogue);
            questDone = true;
        }
        else
            ShowText(questDialogue);
    }

    private void CloseDialogue()
    {
        panelOpen = false;
        SetPanel(false);
    }
    private void ShowText(string t)   { if (dialogueText  != null) dialogueText.text = t; }
    private void SetPanel(bool show)  { if (dialoguePanel != null) dialoguePanel.SetActive(show); }
    private void SetPrompt(bool show) { if (pressEPrompt  != null) pressEPrompt.SetActive(show); }
}