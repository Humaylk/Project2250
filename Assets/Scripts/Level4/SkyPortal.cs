using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyPortal : MonoBehaviour
{
    public SkyGameManager manager;
    public Transform player;
    public float interactDistance = 2f;

    void Update()
    {
        if (player == null)
        {
            SkyPlayerController pc = FindFirstObjectByType<SkyPlayerController>();
            if (pc != null) player = pc.transform;
        }

        if (manager == null)
            manager = FindFirstObjectByType<SkyGameManager>();

        if (player == null || manager == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= interactDistance && manager.levelComplete)
        {
            GameManager.Instance?.uiManager?.ShowHint("Press H to enter Level 5!");

            if (Input.GetKeyDown(KeyCode.H))
            {
                SceneManager.LoadScene("Level5_AetherNexus1");
            }
        }
    }
}
