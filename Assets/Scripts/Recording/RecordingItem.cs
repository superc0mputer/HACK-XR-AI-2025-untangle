using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecordingItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI durationText;
    public Button playButton;

    private AudioClip myClip;
    private AudioSource sharedSource;

    // This function is called by the main recorder when the row is created
    public void Setup(AudioClip clip, string recordingName, AudioSource source)
    {
        myClip = clip;
        sharedSource = source;

        // 1. Set Text
        nameText.text = recordingName;
        
        // 2. Format duration (e.g., "0:12")
        int minutes = (int)(clip.length / 60);
        int seconds = (int)(clip.length % 60);
        durationText.text = string.Format("{0}:{1:00}", minutes, seconds);

        // 3. Setup Button
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        if (sharedSource != null && myClip != null)
        {
            // Stop whatever is playing and play this one
            sharedSource.Stop();
            sharedSource.clip = myClip;
            sharedSource.Play();

            Debug.Log("Playing: " + nameText.text);
        }
    }
}
