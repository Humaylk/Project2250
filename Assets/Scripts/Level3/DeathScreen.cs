using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    public float displayDuration = 2.5f;

    void Awake()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
    }

    void OnEnable()
    {
        PlayerHealth.OnDeath += Show;
    }

    void OnDisable()
    {
        PlayerHealth.OnDeath -= Show;
    }

    public void Show()
    {
        if (deathPanel != null) deathPanel.SetActive(true);
        StartCoroutine(ReloadAfterDelay());
    }

    private IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
