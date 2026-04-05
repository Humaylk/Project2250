using UnityEngine;
using System.Collections.Generic;

// Munadir: Spawns rotating laser beams as colored rectangle sprites
// Munadir: No prefab needed - creates visuals entirely in code
// Munadir: Intensity increases with boss phases - more lasers, faster rotation
public class LaserSystem : MonoBehaviour
{
    [Header("Laser Settings")]
    public int initialLaserCount = 3;
    public float rotationSpeed = 40f;
    public int damagePerHit = 15;
    public float laserLength = 10f;
    public float laserWidth = 0.2f;

    private List<GameObject> activeLasers = new List<GameObject>();
    private bool isRunning = false;

    public void StartLasers()
    {
        isRunning = true;
        SpawnLasers(initialLaserCount);
        Debug.Log("Laser system started with " + initialLaserCount + " lasers.");
    }

    public void StopLasers()
    {
        isRunning = false;
        foreach (GameObject laser in activeLasers)
            if (laser != null) Destroy(laser);
        activeLasers.Clear();
    }

    public void IncreaseIntensity()
    {
        rotationSpeed = 60f;
        SpawnLasers(2);
        Debug.Log("Phase 2 - Laser intensity increased.");
    }

    public void MaxIntensity()
    {
        rotationSpeed = 90f;
        SpawnLasers(2);
        Debug.Log("Phase 3 - Max laser intensity.");
    }

    void Update()
    {
        if (!isRunning) return;
        // Rotate entire LaserSystem object — all child lasers rotate with it
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void SpawnLasers(int count)
    {
        int existingCount = activeLasers.Count;
        float angleStep = 360f / (existingCount + count);

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * (existingCount + i);

            // Create laser as a child of LaserSystem so it rotates with it
            GameObject laser = new GameObject("Laser_" + (existingCount + i));
            laser.transform.SetParent(transform);
            laser.transform.localPosition = Vector3.zero;

            // Rotate this laser to its angle
            laser.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            // Add sprite renderer — bright magenta beam
            SpriteRenderer sr = laser.AddComponent<SpriteRenderer>();
            sr.sprite = GetDefaultSprite();
            sr.color = new Color(1f, 0f, 1f, 0.85f);
            sr.sortingOrder = 5;
// Munadir: Fix for URP - override material to use unlit so laser is always visible
            sr.material = new Material(Shader.Find("Sprites/Default"));

            // Scale: thin and long — this is the beam shape
            laser.transform.localScale = new Vector3(laserWidth, laserLength, 1f);

            // Move it so it extends from center outward
            laser.transform.localPosition = new Vector3(0f, laserLength * 0.5f, 0f);

            // Add trigger collider for damage
            BoxCollider2D col = laser.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1f, 1f); // normalized — actual size from transform scale

            // Add damage script
            LaserDamage ld = laser.AddComponent<LaserDamage>();
            ld.damage = damagePerHit;

            activeLasers.Add(laser);
        }

        Debug.Log("Total lasers: " + activeLasers.Count);
    }

    private Sprite GetDefaultSprite()
    {
        // Munadir: Fixed for URP - use a proper readable texture
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
    }
}