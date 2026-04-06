using UnityEngine;

public class HelmetPickup : MonoBehaviour
{
    public float pickupRange = 2f;
    public RuntimeAnimatorController helmetController;

    private Transform player;
    private Animator playerAnimator;
    private SpriteRenderer helmetSr;
    private bool pickedUp = false;

    void Start()
    {
        helmetSr = GetComponent<SpriteRenderer>();

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            player = pc.transform;
            playerAnimator = pc.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("HelmetPickup: PlayerController not found!");
        }
    }

    void Update()
    {
        if (pickedUp || player == null) return;

        // Only pickable once the helmet has fully faded in (alpha >= 0.9)
        if (helmetSr != null && helmetSr.color.a < 0.9f) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= pickupRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupHelmet();
        }
    }

    void PickupHelmet()
    {
        pickedUp = true;
        gameObject.SetActive(false);

        // Switch player to helmet sprites
        Level3PlayerAppearance appearance = FindFirstObjectByType<Level3PlayerAppearance>();
        if (appearance != null)
            appearance.EquipHelmet();
        else
            Debug.LogWarning("HelmetPickup: Level3PlayerAppearance not found on player.");
        Debug.Log("HelmetPickup: Helmet equipped!");

        WaterIslandLevel level = FindFirstObjectByType<WaterIslandLevel>();
        if (level != null && level.oxygenTimer != null)
        {
            level.oxygenTimer.timeRemaining += 30f;
            level.oxygenTimer.isRunning = true;
            Debug.Log("HelmetPickup: O2 timer set to 45s.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
