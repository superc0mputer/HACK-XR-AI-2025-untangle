using UnityEngine;
using System.Collections;
using TMPro;
using Oculus.Interaction;
using UnityEngine.InputSystem;

public class MemoryBox : MonoBehaviour
{
    [Header("Assign Parts")]
    public GameObject boxVisuals;
    public GameObject journalContent; // The Text object inside

    [Header("Pop Animation")]
    public float popDuration = 0.5f;
    public float floatHeight = 0.3f;  // How high the text floats up

    private bool isOpened = false;
    private Collider boxCollider;
    private Grabbable grabbable;

    void Start()
    {
        boxCollider = GetComponent<Collider>();
        
        // Ensure text is hidden and small at the start
        if (journalContent != null)
        {
            journalContent.SetActive(false);
            journalContent.transform.localScale = Vector3.zero;
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
        if (journalContent != null)
        {
            journalContent.SetActive(true);
            //StartCoroutine(AnimatePop());
        }
    }

    private IEnumerator AnimatePop()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one; // Or whatever your original text scale was (e.g. 0.005)
        
        // Use the current scale of the text as the target if it's not 1
        // (Hackathon tip: Set the scale you want in Scene view, script will respect it)
        endScale = new Vector3(0.005f, 0.005f, 0.005f); 

        Vector3 startPos = journalContent.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, floatHeight, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / popDuration;
            
            // "Overshoot" math for a bouncy pop effect
            float ease = Mathf.Sin(t * Mathf.PI * 0.5f); 

            journalContent.transform.localScale = Vector3.Lerp(startScale, endScale, ease);
            journalContent.transform.localPosition = Vector3.Lerp(startPos, endPos, ease);
            
            yield return null;
        }
    }
    void Update()
    {
        // 2. USE THIS INSTEAD
        // We check if a keyboard is present AND if the space key was pressed
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Debug: Forcing box to Open() with Spacebar.");
            Open();
        }
    }
}