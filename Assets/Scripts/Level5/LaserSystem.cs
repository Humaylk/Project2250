using UnityEngine;
using System.Collections.Generic;

public class LaserSystem : MonoBehaviour
{
    [Header("Settings")]
    public float fireInterval = 3.5f;
    public float rotationSpeed = 40f;
    public float bulletSpeed = 5f;
    public int bulletDamage = 5;
    public float cannonSize = 0.4f;

    private List<LaserCannon> cannons = new List<LaserCannon>();
    private bool isRunning = false;

    public void StartLasers()
    {
        isRunning = true;
        SpawnCannons();
        foreach (LaserCannon c in cannons)
            c.StartFiring();
    }

    public void StopLasers()
    {
        isRunning = false;
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
            if (c != null) c.SetSpeed(60f, 1.2f);
    }

    public void MaxIntensity()
    {
        foreach (LaserCannon c in cannons)
            if (c != null) c.SetSpeed(90f, 0.7f);
    }

    private void SpawnCannons()
    {
        Vector3[] positions = {
            new Vector3(-8.4f,  4.6f, 0f),  // Top Left
            new Vector3( 8.4f,  4.6f, 0f),  // Top Right
            new Vector3(-8.4f, -4.6f, 0f),  // Bottom Left
            new Vector3( 8.4f, -4.6f, 0f)   // Bottom Right
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject cannonObj = new GameObject("LaserCannon_" + i);
            cannonObj.transform.position = positions[i];

            // MUNADIR FIX: Force the cannon to face (0,0) immediately
            Vector3 directionToCenter = Vector3.zero - positions[i];
            float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
            // Subtract 90 because Unity's "Up" is the Y axis
            cannonObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            SpriteRenderer sr = cannonObj.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.8f, 0f, 0.8f, 1f);
            sr.sortingOrder = 6;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sprite = MakeSprite();
            cannonObj.transform.localScale = new Vector3(cannonSize, cannonSize * 2f, 1f);

            LaserCannon cannon = cannonObj.AddComponent<LaserCannon>();
            cannon.rotationRange = 20f; 
            cannon.rotationSpeed = rotationSpeed;
            cannon.fireInterval = fireInterval;
            cannon.bulletSpeed = bulletSpeed;
            cannon.damage = bulletDamage;

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