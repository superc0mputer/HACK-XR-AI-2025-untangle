using UnityEngine;
using System.Collections;
using TMPro;
using Oculus.Interaction;
using UnityEngine.InputSystem;

public class MemoryBox : MonoBehaviour
{
    public JournalEntry journalEntry;

    [Header("Assign Parts")]
    public GameObject panel; // Must be the parent object of the canvas
    public GameObject boxVisuals;
    public TextMeshProUGUI mood;
    public TextMeshProUGUI eventTags;
    public TextMeshProUGUI notes;
    public TextMeshProUGUI emoji;

    [Header("Pop Animation")]
    public float popDuration = 0.5f;
    public float floatHeight = 0.3f;  // How high the text floats up
    public GameObject confetti;
    
    private bool isOpened = false;
    private Collider boxCollider;
    private Grabbable grabbable;

    // This is called by BoxSpawner right after creating the box.
    public void Initialize(JournalEntry entry)
    {
        journalEntry = entry;
        mood.text = journalEntry.Mood;
        eventTags.text = journalEntry.EventTags;
        notes.text = journalEntry.Notes;
        emoji.text = journalEntry.Emoji;
    }

    void Start()
    {
        boxCollider = GetComponent<Collider>();
        
        // Ensure text is hidden and small at the start
        if (panel != null)
        {
            panel.SetActive(false);
            panel.transform.localScale = Vector3.zero;
        }
        
        // 1. Find the Grabbable component on this object
        grabbable = GetComponent<Grabbable>();

        // 2. Listen for the "WhenPointerEventRaised" (which includes Grabbing)
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += OnPointerEvent;
        }
    }
    
    private void OnPointerEvent(PointerEvent evt)
    {
        // 3. Check if the event is a "Select" (Grab)
        if (evt.Type == PointerEventType.Select)
        {
            Open();
        }
    }

    // Call this function from your Interactable Event (Select/Hover)
    public void Open()
    {
        if (isOpened) return;
        isOpened = true;

        // 1. Hide the box immediately
        if (boxVisuals != null)
        {
            boxVisuals.SetActive(false);
        }

        // 2. Disable the collider so we can't click it again
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // 3. Start the "Plop" animation
        if (panel != null)
        {
            panel.SetActive(true);
            StartCoroutine(AnimatePop());
            confetti.SetActive(true);
            
        }
    }

    private IEnumerator AnimatePop()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one; // Or whatever your original text scale was (e.g. 0.005)
        
        // Use the current scale of the text as the target if it's not 1
        // (Hackathon tip: Set the scale you want in Scene view, script will respect it)
        endScale = new Vector3(0.15f, 0.15f, 0.15f); 

        Vector3 startPos = panel.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, floatHeight, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / popDuration;
            
            // "Overshoot" math for a bouncy pop effect
            float ease = Mathf.Sin(t * Mathf.PI * 0.5f); 

            panel.transform.localScale = Vector3.Lerp(startScale, endScale, ease);
            panel.transform.localPosition = Vector3.Lerp(startPos, endPos, ease);
            
            yield return null;
        }
    }
}