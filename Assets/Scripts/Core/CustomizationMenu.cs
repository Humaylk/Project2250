using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject menuPanel;

    [Header("Color Buttons")]
    public Button[] colorButtons;
    public Color[] colorOptions = {
        Color.blue,
        Color.white,
        new Color(0.55f, 0.27f, 0.07f),
        Color.black
    };

    [Header("Style Buttons")]
    public Button[] styleButtons;
    public TMP_Text styleNameText;
    public TMP_Text lockedText;

    [Header("Player")]
    public SpriteRenderer playerSprite;

    private PlayerAppearance appearance;
    private bool menuOpen = false;

    void Start()
    {
        appearance = new PlayerAppearance();

        for (int i = 0; i < colorButtons.Length && i < colorOptions.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() => ApplyColor(index));
            colorButtons[i].GetComponent<Image>().color = colorOptions[i];
        }

        for (int i = 0; i < styleButtons.Length; i++)
        {
            int index = i;
            styleButtons[i].onClick.AddListener(() => ApplyStyle(index));
        }

        if (menuPanel != null)
            menuPanel.SetActive(false);

        RefreshUI();
    }
