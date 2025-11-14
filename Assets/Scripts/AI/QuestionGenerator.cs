using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro

public class QuestionGenerator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionText1;
    public TMP_Text questionText2;
    public TMP_Text questionText3;
    
    [Header("User Info")]
    public TextMeshProUGUI mood;
    public TextMeshProUGUI eventTags;
    public TextMeshProUGUI notes;

    [Header("API Settings")]
    private string apiKey = "put api key here"; 
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    void Start()
    {
        // We strictly tell GPT to return JSON so Unity can read it easily
        string distinctPrompt = $"You are a reflective-support assistant for a VR emotional-processing game. The user provides an emotional entry with the following fields:\n• emotion_label (e.g., sadness, joy, frustration)\n• valence/arousal\n• description (free-text explanation of the event)\n• tags (key themes or contexts)\n\nYour task is to generate brief, personalised reflective questions that help the user understand the emotion and the event behind it. The questions must remain gentle, non-clinical, and non-prescriptive. They should encourage curiosity and emotional awareness rather than advice. Tailor the questions to this mood {mood.text}, these tags {eventTags.text}, and these notes {notes.text} " +
                                "Return ONLY a JSON object with this exact structure: " +
                                "{ \"questions\": [\"Question 1 text\", \"Question 2 text\", \"Question 3 text\"] }";

        StartCoroutine(GetQuestionsRoutine(distinctPrompt));
    }

    IEnumerator GetQuestionsRoutine(string prompt)
    {
        // 1. Setup Request
        ChatCompletionRequest requestData = new ChatCompletionRequest();
        requestData.model = "gpt-4o-mini"; // Fast and cheap
        // Lower temperature = more predictable formatting
        requestData.messages = new List<Message>
        {
            new Message { role = "system", content = "You represent data in JSON format only." },
            new Message { role = "user", content = prompt }
        };

        string json = JsonUtility.ToJson(requestData);

        // 2. Send Request
        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Error: " + webRequest.error);
            }
            else
            {
                HandleResponse(webRequest.downloadHandler.text);
            }
        }
    }

    void HandleResponse(string jsonResponse)
    {
        var responseObj = JsonUtility.FromJson<ChatCompletionResponse>(jsonResponse);
        
        if (responseObj.choices != null && responseObj.choices.Count > 0)
        {
            string content = responseObj.choices[0].message.content;

            // CLEANUP: GPT sometimes puts ```json marks around code. We remove them.
            content = content.Replace("```json", "").Replace("```", "").Trim();

            // 3. Parse the specific list of questions
            try 
            {
                QuestionList data = JsonUtility.FromJson<QuestionList>(content);
                
                // 4. Assign to UI (with safety checks)
                if (data.questions.Length >= 1) questionText1.text = data.questions[0];
                if (data.questions.Length >= 2) questionText2.text = data.questions[1];
                if (data.questions.Length >= 3) questionText3.text = data.questions[2];
            }
            catch (System.Exception e)
            {
                Debug.LogError("Could not parse JSON: " + content + "\nError: " + e.Message);
            }
        }
    }
}