using UnityEngine;

// Munadir: Manages all player abilities unlocked across levels
// Munadir: Teammates add their abilities here when their levels are done
// Munadir: Each ability has a cooldown and damage value
// Munadir: F = Fire ability (Level 2), P = Punch ability (Level 4)
public class AbilityManager : MonoBehaviour
{
    [Header("Fire Ability (unlocked Level 2)")]
    public bool hasFireAbility = false;
    public float fireDamage = 40f;
    public float fireCooldown = 3f;
    private float lastFireTime = -999f;

    [Header("Punch Ability (unlocked Level 4)")]
    public bool hasPunchAbility = false;
    public float punchDamage = 30f;
    public float punchCooldown = 2f;
    private float lastPunchTime = -999f;

    [Header("References")]
    public UIManager uiManager;
    public ElementalBoss boss;

    void Start()
    {
        // Check ProgressionSystem for unlocked abilities
        ProgressionSystem ps = FindFirstObjectByType<ProgressionSystem>();
        if (ps != null)
        {
            hasFireAbility = ps.HasAbility("Fire");
            hasPunchAbility = ps.HasAbility("Punch");
        }

        // For testing Level 5 standalone - enable both
        // Remove these two lines once teammates finish their levels
        hasFireAbility = true;
        hasPunchAbility = true;

        Debug.Log("AbilityManager initialized. Fire: " + hasFireAbility + " Punch: " + hasPunchAbility);
    }

    void Update()
    {
        // F key = Fire ability
        if (Input.GetKeyDown(KeyCode.F))
            UseFireAbility();

        // P key = Punch ability
        if (Input.GetKeyDown(KeyCode.P))
            UsePunchAbility();
    }

    private void UseFireAbility()
    {
        if (!hasFireAbility)
        {
            uiManager?.ShowHint("Fire ability not unlocked yet!");
            return;
        }

        if (Time.time - lastFireTime < fireCooldown)
        {
            float remaining = fireCooldown - (Time.time - lastFireTime);
            uiManager?.ShowHint("Fire ability cooling down: " + remaining.ToString("F1") + "s");
            return;
        }

        lastFireTime = Time.time;
        Debug.Log("FIRE ABILITY USED!");

        // Deal damage to boss if in range
        if (boss != null)
        {
            float dist = Vector2.Distance(transform.position, boss.transform.position);
            if (dist <= 8f) // fire has long range
            {
                boss.TakeDamage((int)fireDamage);
                uiManager?.ShowHint("Fire ability hit boss for " + fireDamage + " damage!");
                GameManager.Instance?.progressionSystem?.AddCombatXP(15);
            }
            else
            {
                uiManager?.ShowHint("Too far from boss for Fire ability!");
            }
        }
    }

    private void UsePunchAbility()
    {
        if (!hasPunchAbility)
        {
            uiManager?.ShowHint("Punch ability not unlocked yet!");
            return;
        }

        if (Time.time - lastPunchTime < punchCooldown)
        {
            float remaining = punchCooldown - (Time.time - lastPunchTime);
            uiManager?.ShowHint("Punch cooling down: " + remaining.ToString("F1") + "s");
            return;
        }

        lastPunchTime = Time.time;
        Debug.Log("PUNCH ABILITY USED!");

        // Punch has short range but high damage
        if (boss != null)
        {
            float dist = Vector2.Distance(transform.position, boss.transform.position);
            if (dist <= 3f)
            {
                boss.TakeDamage((int)punchDamage);
                uiManager?.ShowHint("PUNCH hit boss for " + punchDamage + " damage!");
                GameManager.Instance?.progressionSystem?.AddCombatXP(10);
            }
            else
            {
                uiManager?.ShowHint("Get closer to use Punch ability!");
            }
        }
    }

    // Called by teammates when they unlock abilities in their levels
    public void UnlockFireAbility()
    {
        hasFireAbility = true;
        GameManager.Instance?.progressionSystem?.UnlockAbility("Fire");
        uiManager?.ShowHint("Fire ability unlocked!");
    }

    public void UnlockPunchAbility()
    {
        hasPunchAbility = true;
        GameManager.Instance?.progressionSystem?.UnlockAbility("Punch");
        uiManager?.ShowHint("Punch ability unlocked!");
    }
}