using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to the exit door GameObject in Level 3.
// Press H when nearby to load Level4_Sky.
// Call Open() from WaterIslandLevel once all tasks are done.
public class Level3ExitDoor : MonoBehaviour
{
    public float interactDistance = 5f;
    public bool isOpen = false;

    private Transform _player;

    void Update()
    {
        // Find player by PlayerController component — more reliable than tag
        if (_player == null)
        {
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null)
            {
                _player = pc.transform;
                Debug.Log("[Level3ExitDoor] Found player: " + _player.name);
            }
        }

        if (_player == null)
        {
            if (Input.GetKeyDown(KeyCode.H))
                Debug.LogWarning("[Level3ExitDoor] H pressed but player not found!");
            return;
        }

        float dist = Vector2.Distance(transform.position, _player.position);

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("[Level3ExitDoor] H pressed | dist=" + dist + " | isOpen=" + isOpen);

            if (!isOpen)
            {
                GameManager.Instance?.uiManager?.ShowHint("Complete your objective to open the exit.");
                return;
            }

            if (dist > interactDistance)
            {
                GameManager.Instance?.uiManager?.ShowHint("Get closer to the exit door!");
                return;
            }

            Debug.Log("[Level3ExitDoor] Loading Level4_Sky");
            int idx = SceneUtility.GetBuildIndexByScenePath("Level4_Sky");
            if (idx < 0)
            {
                // Try full path format
                Debug.LogError("[Level3ExitDoor] 'Level4_Sky' not found in Build Settings! Add it via File > Build Settings.");
                // Attempt load anyway in case it is there under a different path
                SceneManager.LoadScene("Level4_Sky");
            }
            else
            {
                SceneManager.LoadScene(idx);
            }
        }

        // Hint when nearby
        if (dist <= interactDistance && isOpen)
            GameManager.Instance?.uiManager?.ShowHint("Press H to go to Level 4!");
    }

    public void Open()
    {
        isOpen = true;
        Debug.Log("[Level3ExitDoor] Exit door is now OPEN.");
        GameManager.Instance?.uiManager?.ShowHint("Exit is open! Press H at the door to advance.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isOpen ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
