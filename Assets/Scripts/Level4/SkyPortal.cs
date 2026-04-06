using UnityEngine;

public class SkyPortal : MonoBehaviour
{
    public SkyGameManager manager;
    public Transform player;

    public float interactDistance = 2f;

    void Update()
    {
        if (manager == null || player == null)
        {
            Debug.Log("Missing references");
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        Debug.Log("Distance to portal: " + dist + 
                  " | LevelComplete: " + manager.levelComplete);

        if (dist < interactDistance && manager.levelComplete)
        {
            Debug.Log("READY TO ENTER PORTAL");

            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("LOADING NEXT LEVEL");
                GameManager.Instance?.AdvanceLevel();
            }
        }
    }
}