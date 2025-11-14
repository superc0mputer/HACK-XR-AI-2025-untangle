using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(AudioSource))]
public class PlaybackRecorder : MonoBehaviour
{
    [Header("UI References")]
    public Button actionButton;
    public TextMeshProUGUI buttonText;

    [Header("Settings")]
    public Color idleColor = Color.green;
    public Color recordColor = Color.red;
    public Color playingColor = Color.cyan;

    private AudioSource audioSource;
    private AudioClip recordedClip;
    private string micDevice;
    private bool isRecording = false;
    
    // Prevent double-clicking the button too fast
    private bool interactionCooldown = false; 

    private const int MAX_BUFFER = 60; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 1. Request Permissions (CRITICAL FOR QUEST)
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        #endif

        // 2. Find Microphone
        if (Microphone.devices.Length > 0)
        {
            // Often device[0] on Quest is correct, but verify in logs
            micDevice = Microphone.devices[0];
            Debug.Log($"Microphone Found: {micDevice}");
        }
        else
        {
            buttonText.text = "No Mic";
            actionButton.interactable = false;
        }

        actionButton.onClick.AddListener(OnButtonClick);
        SetStateIdle();
    }

    private void OnButtonClick()
    {
        // Prevent spamming the button (Debounce)
        if (interactionCooldown) return;
        StartCoroutine(CooldownRoutine());

        if (isRecording)
        {
            StopAndPlay();
        }
        else
        {
            // If audio is playing, stop it immediately
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            // CRITICAL: Cancel any pending resets from previous playbacks
            CancelInvoke("SetStateIdle");
            
            StartRecording();
        }
    }

    private void StartRecording()
    {
        if (micDevice == null) return;

        // Stop any previous recording cleanly
        Microphone.End(micDevice);

        // Start recording
        // Note: On Quest, ensure Project Settings > Audio > DSP Buffer Size is 'Best Latency'
        recordedClip = Microphone.Start(micDevice, false, MAX_BUFFER, 44100);
        isRecording = true;

        // Visuals
        UpdateUI(recordColor, "Stop & Play");
        Debug.Log("Recording Started...");
    }

    private void StopAndPlay()
    {
        if (!isRecording) return;

        // 1. Stop Mic
        int position = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        isRecording = false;

        // 2. Trim and Play
        if (position > 0 && recordedClip != null)
        {
            recordedClip = TrimClip(recordedClip, position);
            
            audioSource.clip = recordedClip;
            audioSource.Play();
            
            UpdateUI(playingColor, "Playing...");
            Debug.Log($"Playing clip. Length: {recordedClip.length}");

            // Reset to idle only after this specific clip finishes
            Invoke("SetStateIdle", recordedClip.length);
        }
        else
        {
            Debug.LogWarning("Recording failed or was too short.");
            SetStateIdle();
        }
    }

    private void SetStateIdle()
    {
        // Only reset if we aren't currently recording or manually stopped
        if (!isRecording)
        {
            UpdateUI(idleColor, "Start Recording");
        }
    }

    private void UpdateUI(Color color, string text)
    {
        if(actionButton != null) actionButton.image.color = color;
        if(buttonText != null) buttonText.text = text;
    }

    // Adds a small delay so you can't double click the button by accident
    private IEnumerator CooldownRoutine()
    {
        interactionCooldown = true;
        yield return new WaitForSeconds(0.5f);
        interactionCooldown = false;
    }

    private AudioClip TrimClip(AudioClip original, int position)
    {
        // Safety check
        if (position == 0) return original;

        float[] data = new float[position * original.channels];
        original.GetData(data, 0);
        
        AudioClip newClip = AudioClip.Create(original.name, position, original.channels, original.frequency, false);
        newClip.SetData(data, 0);
        return newClip;
    }
}