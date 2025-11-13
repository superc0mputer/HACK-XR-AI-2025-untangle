using UnityEngine;
using System.Collections;

public class MemoryBox : MonoBehaviour
{
    [Header("Assign Parts")] 
    public Transform lidObject; // Assign 'Lid" object here
    public GameObject journalContent; //Assign "Text" object here

    [Header("Settings")] 
    public float openSpeed = 2.0f;
    public float openAngle = -110f;

    private bool isOpen = false;

    public void Open()
    {
        if (isOpen) return; //Do not open twice

        StartCoroutine(AnimateLid());
    }

    private IEnumerator AnimateLid()
    {
        isOpen = true;
        Quaternion startRot = lidObject.localRotation;
        Quaternion endRot = Quaternion.Euler(openAngle, 0, 0); // Target rotation

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            //Smoothly rotate the lid
            lidObject.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        
        //Once fully open, reveal the text
        if (journalContent != null)
        {
            journalContent.SetActive(true);
        }
    }


}
