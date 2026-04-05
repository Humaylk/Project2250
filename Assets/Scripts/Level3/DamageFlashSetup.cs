using UnityEngine;

public class DamageFlashSetup : MonoBehaviour
{
    public Sprite damageSprite;

    void Start()
    {
        UIManager ui = GameManager.Instance?.uiManager
                    ?? FindFirstObjectByType<UIManager>();
        if (ui != null && damageSprite != null)
            ui.damageSprite = damageSprite;
    }
}
