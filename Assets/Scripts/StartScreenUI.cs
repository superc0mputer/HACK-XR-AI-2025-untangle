using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StartScreen : MonoBehaviour
{
    [Header("Main UI References")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI phrase;
    public Button startButton;
    public string username = "Alex";
    
    public BoxSpawner boxSpawner;

    // --- SECTION 1: IMAGE SWAPPING ---
    [System.Serializable]
    public class DayNightImage
    {
        public Image targetImage;     // The Image Component
        public Sprite daySprite;      // Sprite for Day
        public Sprite nightSprite;    // Sprite for Night
    }

    // --- SECTION 2: TEXT COLOR SWAPPING ---
    [System.Serializable]
    public class DayNightText
    {
        public TextMeshProUGUI targetText; // The Text Component
        public Color dayColor = Color.black; // Default to black
        public Color nightColor = Color.white; // Default to white
    }

    [Header("Theme Settings")]
    public List<DayNightImage> themedImages; // Drag images here
    public List<DayNightText> themedTexts;   // Drag text here

    // ------------------------------------

    void Start()
    {
        UpdateUI();
        startButton.onClick.AddListener(OnStartButtonPressed);
    }

    void UpdateUI()
    {
        DateTime now = DateTime.Now;

        // --- A. LOGIC: Update Strings ---
        string greeting = "Good evening";
        if (now.Hour < 12) greeting = "Good morning";
        else if (now.Hour < 18) greeting = "Good afternoon";

        string date = now.ToString("dddd, MMMM dd");
        string time = now.ToString("h:mm tt");

        clockText.text = $"{time}\n";
        dateText.text = $"Today is {date}\n";
        welcomeText.text = $"{greeting}, {username}\n";
        phrase.text = $"Are you ready to get started?";

        // --- B. LOGIC: Determine Time of Day ---
        // Night is 6 PM (18) through 5:59 AM
        bool isNight = now.Hour >= 18 || now.Hour < 6;

        // --- C. APPLY: Image Sprites ---
        foreach (var item in themedImages)
        {
            if (item.targetImage != null)
            {
                Sprite spriteToUse = isNight ? item.nightSprite : item.daySprite;
                if (spriteToUse != null) item.targetImage.sprite = spriteToUse;
            }
        }

        // --- D. APPLY: Text Colors ---
        foreach (var item in themedTexts)
        {
            if (item.targetText != null)
            {
                Color colorToUse = isNight ? item.nightColor : item.dayColor;
                item.targetText.color = colorToUse;
            }
        }
    }

    void OnStartButtonPressed()
    {
        gameObject.SetActive(false);
        boxSpawner.SpawnBoxes();
    }
}