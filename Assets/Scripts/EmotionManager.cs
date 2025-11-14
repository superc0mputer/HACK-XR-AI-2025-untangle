using System.Collections.Generic;
using UnityEngine;
using System; // For case-insensitive matching

public class EmotionManager : MonoBehaviour
{
    public static EmotionManager Instance; // Singleton for easy access
    public TextAsset csvFile; // Drag your emotion_colors.csv here

    // Dictionary for fast lookups (StringComparer handles "joyful" vs "Joyful")
    private Dictionary<string, Color> colorMap = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

    void Awake()
    {
        // Singleton Setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadColors();
    }

    void LoadColors()
    {
        if (csvFile == null) return;

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Skip header row
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 2)
            {
                string emotionName = data[0].Trim();
                string hexColor = data[1].Trim();

                // Convert Hex to Unity Color
                if (ColorUtility.TryParseHtmlString(hexColor, out Color finalColor))
                {
                    if (!colorMap.ContainsKey(emotionName))
                    {
                        colorMap.Add(emotionName, finalColor);
                    }
                }
            }
        }
    }

    // The helper function you call
    public Color GetColor(string emotion)
    {
        // Trim whitespace just in case (" Joyful " -> "Joyful")
        string cleanEmotion = emotion.Trim();

        if (colorMap.TryGetValue(cleanEmotion, out Color foundColor))
        {
            return foundColor;
        }

        // Default to white if emotion is not in the CSV
        return Color.white; 
    }
}