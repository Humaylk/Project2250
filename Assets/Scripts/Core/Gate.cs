using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Munadir: Updated Gate to implement IInteractable interface
// Munadir: Added H key interaction for player to advance level when gate is open
// Munadir: Added proximity detection so hints show when player is nearby
public class Gate : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    [SerializeField] protected bool blocksCollisionWhenClosed = true;

    [Header("Level Transition")]
    public GameObject gateWall;
    public float levelCompleteDelay = 2f;

    [Header("UI")]
    public GameObject      dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject      pressHPrompt;

    protected Collider2D gateCollider;
    protected Animator   animator;

    private bool playerNearby  = false;
    private bool levelComplete = false;

    protected virtual void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
        animator     = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        ResetGate();
        SetPanel(false);
        SetPrompt(false);
    }

    void Update()
    {
        if (playerNearby && isOpen && !levelComplete && Input.GetKeyDown(KeyCode.H))
        {
            levelComplete = true;
            SetPrompt(false);
            SetPanel(true);
            ShowText("Level Complete!\n+5 HP Gained");
            StartCoroutine(LoadNextLevel());
        }
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelCompleteDelay);
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);
        if (gateWall != null) gateWall.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public virtual void OpenGate()
    {
        isOpen = true;

        if (gateCollider != null && blocksCollisionWhenClosed)
            gateCollider.isTrigger = true;

        if (animator != null)
            animator.SetBool("IsOpen", true);

        Debug.Log(gameObject.name + ": Gate is now OPEN.");
    }

    public virtual void CloseGate()
    {
        isOpen = false;

        if (gateCollider != null && blocksCollisionWhenClosed)
            gateCollider.isTrigger = false;

        if (animator != null)
            animator.SetBool("IsOpen", false);

        Debug.Log(gameObject.name + ": Gate is now CLOSED.");
    }

    public virtual void ResetGate() => CloseGate();
    public bool CanPassThrough()    => isOpen;

    public void Interact()
    {
        if (!isOpen)
            ShowText("Gate is sealed. Collect all items first!");
    }

    public string GetInteractPrompt()
        => isOpen ? "Press H to advance" : "Gate is sealed";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (isOpen)
                SetPrompt(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            SetPrompt(false);
            SetPanel(false);
            levelComplete = false;
        }
    }

    private void ShowText(string t)   { if (dialogueText  != null) dialogueText.text = t; }
    private void SetPanel(bool show)  { if (dialoguePanel != null) dialoguePanel.SetActive(show); }
    private void SetPrompt(bool show) { if (pressHPrompt  != null) pressHPrompt.SetActive(show); }
}