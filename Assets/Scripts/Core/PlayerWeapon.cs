using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public enum WeaponType
    {
        Wooden,
        Metal
    }

    public WeaponType currentWeapon = WeaponType.Wooden;

    public int woodenDamage = 10;
    public int metalDamage = 15;

    public int GetDamage()
    {
        if (currentWeapon == WeaponType.Wooden)
            return woodenDamage;
        else
            return metalDamage;
    }

    public void UpgradeWeapon()
    {
        currentWeapon = WeaponType.Metal;
        Debug.Log("Upgraded to METAL SWORD!");
    }
}