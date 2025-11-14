using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
public class DayNightHandler : MonoBehaviour
{
    // --- CONFIGURATION STRUCTS ---
    [System.Serializable]
    public class DayNightImage
    {
        public Image targetImage;
        public Sprite daySprite;
        public Sprite nightSprite;
    }

    [System.Serializable]
    public class DayNightText
    {
        public TextMeshProUGUI targetText;
        public Color dayColor = Color.black;
        public Color nightColor = Color.white;
    }

    [Header("Theme Lists")]
    public List<DayNightImage> themedImages;
    public List<DayNightText> themedTexts;

    [Header("Debug")]
    public bool forceUpdateNow; // Check this in inspector to test without playing

    private void OnEnable()
    {
        // We use OnEnable so if you hide/unhide the menu, it updates the theme immediately
        ApplyTheme();
    }

    private void OnValidate()
    {
        // Allows you to test the theme in the editor by clicking "Force Update Now"
        if (forceUpdateNow)
        {
            forceUpdateNow = false;
            ApplyTheme();
        }
    }

    public void ApplyTheme()
    {
        DateTime now = DateTime.Now;
        
        // Define Night: 6 PM (18) to 6 AM (6)
        bool isNight = now.Hour >= 18 || now.Hour < 6;

        // 1. Apply Images
        foreach (var item in themedImages)
        {
            if (item.targetImage != null)
            {
                Sprite spriteToUse = isNight ? item.nightSprite : item.daySprite;
                if (spriteToUse != null) item.targetImage.sprite = spriteToUse;
            }
        }

        // 2. Apply Text Colors
        foreach (var item in themedTexts)
        {
            if (item.targetText != null)
            {
                Color colorToUse = isNight ? item.nightColor : item.dayColor;
                item.targetText.color = colorToUse;
            }
        }
    }
}
