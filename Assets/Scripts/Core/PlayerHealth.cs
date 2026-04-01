using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    void Start()
    {
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player Health: " + health);
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
       //Humayl testing:// Debug.Log("PlayerHealth Start Running");

        if (health <= 0)
        {
            Debug.Log("Player Died!");
            GameManager.Instance?.ResetOnDeath();
        }
    }
    
}