using UnityEngine;
using System.Collections.Generic;

// Munadir: Manages 2 laser cannons positioned at top corners of arena
// Munadir: Reduced from 4 to 2 for better gameplay balance
// Munadir: Cannons sweep and fire projectile bullets toward center
// Munadir: Phase 2/3 increase speed and fire rate
public class LaserSystem : MonoBehaviour
{
    [Header("Settings")]
    public float fireInterval = 4f;
    public float rotationSpeed = 30f;
    public float bulletSpeed = 4f;
    public int bulletDamage = 5;
    public float cannonSize = 0.5f;

    [Header("Bullet Visual")]
    public Sprite bulletSprite;

    private List<LaserCannon> cannons = new List<LaserCannon>();

    public void StartLasers()
    {
        SpawnCannons();
        foreach (LaserCannon c in cannons)
            c.StartFiring();
    }

    public void StopLasers()
    {
        foreach (LaserCannon c in cannons)
        {
            if (c != null)
            {
                c.StopFiring();
                Destroy(c.gameObject);
            }
        }
        cannons.Clear();
    }

    public void IncreaseIntensity()
    {
        foreach (LaserCannon c in cannons)
            if (c != null) c.SetSpeed(45f, 2.5f);
    }

    public void MaxIntensity()
    {
        foreach (LaserCannon c in cannons)
            if (c != null) c.SetSpeed(60f, 1.5f);
    }

    private void SpawnCannons()
    {
        // Munadir: Only 2 cannons — top-left and top-right
        Vector3[] positions = {
            new Vector3(-8.4f,  4.6f, 0f),
            new Vector3( 8.4f,  4.6f, 0f)
        };

        for (int i = 0; i < 2; i++)
        {
            GameObject cannonObj = new GameObject("LaserCannon_" + i);
            cannonObj.transform.position = positions[i];

            // Munadir: Face toward center
            Vector3 directionToCenter = Vector3.zero - positions[i];
            float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
            cannonObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            SpriteRenderer sr = cannonObj.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.8f, 0f, 0.8f, 1f);
            sr.sortingOrder = 6;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sprite = MakeSprite();
            cannonObj.transform.localScale = new Vector3(cannonSize, cannonSize * 2f, 1f);

            LaserCannon cannon = cannonObj.AddComponent<LaserCannon>();
            cannon.rotationRange = 25f;
            cannon.rotationSpeed = rotationSpeed;
            cannon.fireInterval = fireInterval;
            cannon.bulletSpeed = bulletSpeed;
            cannon.damage = bulletDamage;
            cannon.bulletSprite = bulletSprite;

            cannons.Add(cannon);
        }
    }

    private Sprite MakeSprite()
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
    }
}
