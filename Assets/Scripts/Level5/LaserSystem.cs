using UnityEngine;
using System.Collections.Generic;

// Munadir: Manages 4 laser cannons positioned around the arena
// Munadir: 2 at top, 2 at bottom, all rotate and fire projectile bullets
// Munadir: Phase 2 and 3 increase fire rate and rotation speed
public class LaserSystem : MonoBehaviour
{
    [Header("Settings")]
    public float fireInterval = 2f;
    public float rotationSpeed = 30f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 15;
    public float cannonSize = 0.4f;

    private List<LaserCannon> cannons = new List<LaserCannon>();
    private bool isRunning = false;

    public void StartLasers()
    {
        isRunning = true;
        SpawnCannons();
        foreach (LaserCannon c in cannons)
            c.StartFiring();
        Debug.Log("4 laser cannons started.");
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
            if (c != null) c.SetSpeed(50f);
        Debug.Log("Phase 2 - Cannons firing faster.");
    }

    public void MaxIntensity()
    {
        foreach (LaserCannon c in cannons)
            if (c != null) c.SetSpeed(80f);
        Debug.Log("Phase 3 - Max cannon intensity.");
    }

    private void SpawnCannons()
    {
        // 4 cannon positions: top-left, top-right, bottom-left, bottom-right
        Vector3[] positions = {
            new Vector3(-3f,  5f, 0f),  // top left
            new Vector3( 3f,  5f, 0f),  // top right
            new Vector3(-3f, -5f, 0f),  // bottom left
            new Vector3( 3f, -5f, 0f)   // bottom right
        };

        // Starting angles — top cannons aim down, bottom cannons aim up
        float[] startAngles = { 225f, 315f, 135f, 45f };

        for (int i = 0; i < 4; i++)
        {
            GameObject cannonObj = new GameObject("LaserCannon_" + i);
            cannonObj.transform.position = positions[i];
            cannonObj.transform.rotation = Quaternion.Euler(0, 0, startAngles[i]);

            // Visual for cannon body
            SpriteRenderer sr = cannonObj.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.8f, 0f, 0.8f, 1f);
            sr.sortingOrder = 6;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sprite = MakeSprite();
            cannonObj.transform.localScale = new Vector3(cannonSize, cannonSize * 2f, 1f);

            LaserCannon cannon = cannonObj.AddComponent<LaserCannon>();
            cannon.rotationRange = 45f;
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