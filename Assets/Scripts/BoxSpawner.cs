using UnityEngine;
using Meta.XR.MRUtilityKit; // <-- The MRUK script
using System.Collections.Generic; // For the list

public class BoxSpawner : MonoBehaviour
{
    [Header("Player")]
    public Transform centerEyeAnchor; // Drag OVRCameraRig/TrackingSpace/CenterEyeAnchor

    [Header("Spawning")]
    public GameObject memoryBoxPrefab;
    public float spawnRadius = 1.0f; // 1 meter circle
    public float spawnHeightOffset = 0.1f; // 10cm *above* the floor

    // Hardcoded journal entries
    private List<string> journalEntries = new List<string>
    {
        "Felt stressed during the team meeting.",
        "Had a really nice lunch with Sarah.",
        "Worried about the deadline for Project X."
    };

    // This is the public function our StartScreen will call
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

            Instantiate(memoryBoxPrefab, spawnPosition, spawnRotation);
        }
    }
}
