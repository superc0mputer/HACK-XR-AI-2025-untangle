using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StartScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI phrase;
    public Button startButton;
    public string username = "Alex";
    
    public BoxSpawner boxSpawner;

    void Start()
    {
        UpdateText();
        startButton.onClick.AddListener(OnStartButtonPressed);
    }

    void UpdateText()
    {
        DateTime now = DateTime.Now;
        string greeting = "Good evening,";
        if (now.Hour < 12) greeting = "Good morning";
        else if (now.Hour < 18) greeting = "Good afternoon";

        string date = now.ToString("dddd, MMMM dd");
        string time = now.ToString("h:mm tt");

        clockText.text = $"{time}\n";
        dateText.text = $"Today is {date}\n";
        welcomeText.text = $"{greeting}, {username}\n";
        phrase.text = $"You have 6 logs from today.\n" +
                      $"Ready to untangle?";
    }

    void OnStartButtonPressed()
    {
        // Hide this UI immediately
        gameObject.SetActive(false);

        boxSpawner.SpawnBoxes();
    }
}