using TMPro;
using UnityEngine;

public class PutQuestionOnButtonPress : MonoBehaviour
{
    // 1. Define the slots here
    public TextMeshProUGUI sourceText;
    public TextMeshProUGUI destinationText;

    // 2. Remove parameters from the method
    public void OnButtonPress()
    {
        // 3. Use the variables defined above
        if(sourceText != null && destinationText != null)
        {
            destinationText.text = sourceText.text;
        }
    }
}