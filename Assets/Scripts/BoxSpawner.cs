using UnityEngine;
using Meta.XR.MRUtilityKit; // <-- The MRUK script
using System.Collections.Generic;
using TMPro; // For the list

public class BoxSpawner : MonoBehaviour
{
    [Header("Player")]
    public Transform centerEyeAnchor; // Drag OVRCameraRig/TrackingSpace/CenterEyeAnchor

    [Header("Spawning")]
    public GameObject memoryBoxPrefab;
    public float spawnRadius = 1.0f; // 1 meter circle
    public float spawnHeightOffset = 0.1f; // 10cm *above* the floor

    [Tooltip("Drag your journal_data.csv file here")]
    public TextAsset journalCSV;
    
    // This holds our structred data from the CVS
    private List<JournalEntry> journalEntries = new List<JournalEntry>();

    void Awake()
    {
        // Parse the CVS file when the script first loads
        LoadJournalData();
    }

    private void LoadJournalData()
    {
        if (journalCSV == null)
        {
            return;
        }
        
        // Split the file into lines
        string[] lines = journalCSV.text.Split('\n');
        
        // Start from i=1 to skip the header row
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            // Split the line by commas
            // No "Commas" in the "Notes field or this will not work
            string[] fields = line.Split(',');

            if (fields.Length >= 4)
            {
                // Create a new JournalEntry object and add it to our list
                // Assumes order: Mood, EventTags, Notes, Emoji
                JournalEntry entry = new JournalEntry(fields[0], fields[1], fields[2], fields[3]);
                journalEntries.Add(entry);
            }
        }
        Debug.Log($"Loaded {journalEntries.Count} journal entries from CSV.");
    }

    // This is the public function StartScreen will call
    public void SpawnBoxes()
    {
        float floorY = 0f; // Default floor height

        // --- 1. FIND THE ROOM ---
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room != null && room.FloorAnchor != null)
        {
            // --- 2. FIND THE FLOOR ---
            // If we found a room AND it has a floor, get its height
            floorY = room.FloorAnchor.transform.position.y;
            Debug.Log("SUCCESS: Found floor at Y = " + floorY);
        }
        else
        {
            // This is the fallback
            Debug.LogWarning("WARNING: Could not find MRUK floor. Spawning at Y=0.");
        }

        // --- 3. FIND THE PLAYER ---
        // Get player's (X, Z) position and pair it with the *real* floor's Y
        Vector3 playerFloorPosition = centerEyeAnchor.position;
        playerFloorPosition.y = floorY + spawnHeightOffset;

        // --- 4. SPAWN IN A CIRCLE ---
        for (int i = 0; i < journalEntries.Count; i++)
        {
            float angle = i * (360f / journalEntries.Count);
            float radians = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * spawnRadius;
            float z = Mathf.Sin(radians) * spawnRadius;

            Vector3 spawnPosition = playerFloorPosition + new Vector3(x, 0, z);
            
            // Make the box look at the player
            Vector3 lookAtPos = playerFloorPosition;
            lookAtPos.y = spawnPosition.y; // Keep box level
            Quaternion spawnRotation = Quaternion.LookRotation(spawnPosition - lookAtPos);
            
            // 1. Instantiate the box and keep a reference to it
            GameObject newBox = Instantiate(memoryBoxPrefab, spawnPosition, spawnRotation);
            
            // 2. Try to get the MemoryBox script from the new instance
            MemoryBox memoryBoxScript = newBox.GetComponent<MemoryBox>();
            
            // 3. If the script exists, call a function to set its text
            if (memoryBoxScript != null)
            {
                // Pass the correct journal entry from our list
                memoryBoxScript.Initialize(journalEntries[i]);
            }
            else
            {
                Debug.LogWarning("Spawned a memoryBoxPrefab, but it's missing a 'MemoryBox.cs' script!");
            }
        }
    }
}
