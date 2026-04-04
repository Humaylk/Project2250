using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("YOU WIN!");

            PlayerWeapon pw = other.GetComponent<PlayerWeapon>();
            if (pw != null)
            {
                pw.UpgradeWeapon();
            }

            Time.timeScale = 0f;
        }
    }
}