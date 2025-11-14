using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this namespace if using TextMeshPro

public class SyncColorToText : MonoBehaviour
{
    public TMP_Text emotionLabel; // Drag the Text component that says "Joyful" here
    public Image imageToColor;    // Drag the Image you want to color here

    // Optional: Run this when the game starts
    void Start()
    {
        UpdateColor();
    }

    // Call this function whenever you change the text
    public void UpdateColor()
    {
        if (EmotionManager.Instance == null) return;

        // 1. Get the text (e.g., "Joyful")
        string currentText = emotionLabel.text;

        // 2. Look up the color
        Color newColor = EmotionManager.Instance.GetColor(currentText);

        // 3. Apply it
        imageToColor.color = newColor;
    }

    // If your text changes frequently and you don't want to call UpdateColor() manually,
    // you can uncomment this Update loop (easier but slightly less efficient):
    /*
    void Update()
    {
        UpdateColor();
    }
    */
}