using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement; 


public class CanvasText : MonoBehaviour
{
    public Canvas canvas; 
    private Text displayText;

    private const string apiUrl = "Your API URL"; // The API endpoint
    public cube1script cubeScript;
    public cube2script cubeScript2;
    public cube3script cubeScript3;
    public restartScript restartscript;
    public exitScript exitscript;

    private string endanswer = "";
     public GameObject cube1; 
          public GameObject cube2; 
     public GameObject cube3; 
          public GameObject desk; 
     public GameObject restartobj; 
          public GameObject exitobj; 

   string answer = "";
        string answertext = "";

    public static int playAttempts = 0; 
    public static int correct = 0; 
    public static int incorrect = 0; 

    void Start()
    {
                // Make the API call
        StartCoroutine(SendRequest());
        cube1.SetActive(false);
        cube2.SetActive(false);
        cube3.SetActive(false);
        desk.SetActive(false);
         restartobj.SetActive(false);
            exitobj.SetActive(false);
   
            answer = "";
         answertext = "";

        // Increment play attempts each time the game starts
        playAttempts++;
    }

     void Update()
    {

         if (restartscript != null && restartscript.restartgrabbed)
        {
                        restartscript.restartgrabbed = false;

            RestartGame();
        }
        if (exitscript != null && exitscript.exitgrabbed)
        {
                        exitscript.exitgrabbed = false;

            ExitGame();
        }    

        if (cubeScript != null && cubeScript.grabbed)
        {

            if(answer == "red" || answer == "Red")
            {

               Debug.Log("Correct! The answer was:" + answertext); 
                   displayText.text = "Correct! The answer was:" + answertext;
                correct++;
            }
            else{
            
            Debug.Log("Incorrect! The answer was:" + answertext);      
                               displayText.text = "Incorrect! The answer was:" + answertext;
                incorrect++;


            }
            cubeScript.grabbed = false;
            ShowPopUp();

        }
               if (cubeScript2 != null && cubeScript2.grabbed)
        {

            if(answer == "green" || answer == "Green")
            {

               Debug.Log("Correct! The answer was:" + answertext);   
                                  displayText.text = "Correct! The answer was:" + answertext;
                correct++;
            }
            else{
            
            Debug.Log("Incorrect! The answer was:" + answertext);     
                                           displayText.text = "Incorrect! The answer was:" + answertext;
 
                incorrect++;
            }
                        cubeScript2.grabbed = false;
                                    ShowPopUp();


        }
               if (cubeScript3 != null && cubeScript3.grabbed)
        {

            if(answer == "blue" || answer == "Blue")
            {

               Debug.Log("Correct! The answer was:" + answertext);    
                                  displayText.text = "Correct! The answer was:" + answertext;
                correct++;
            }
            else{
            
            Debug.Log("Incorrect! The answer was:" + answertext);   
                                           displayText.text = "Incorrect! The answer was:" + answertext;
                   incorrect++;


            }
                        cubeScript3.grabbed = false;
                                    ShowPopUp();


        }

    }
     void ShowPopUp()
    {
           restartobj.SetActive(true);
            exitobj.SetActive(true);
              cube1.SetActive(false);
        cube2.SetActive(false);
        cube3.SetActive(false);
    }

    void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ExitGame()
    {
              restartobj.SetActive(false);
            exitobj.SetActive(false);
        // Display the number of attempts
        displayText.text = "Number of play attempts: " + playAttempts + "|| Correct: " + correct + "|| Incorrect: " + incorrect;


    }

    private IEnumerator SendRequest()
    {
        // Prepare the request data
        string jsonData = "{\"messages\": [{\"role\": \"user\", \"content\": \"Write a short, engaging story in 100 words or less.\"}], \"model\": \"Llama-3.3-70b-Versatile\"}";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(apiUrl, jsonData))
        {
            // Set request headers
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.SetRequestHeader("Authorization", "Bearer " + GetApiKey());
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Attach the JSON data
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;

             // Parse the response
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseText);

                    // Extract content from the response
                    if (responseData != null && responseData.choices != null && responseData.choices.Length > 0)
                    {
                        string content = responseData.choices[0].message.content;
                        Debug.Log($"Extracted Content: {content}");
                                            
                            // Create a Text object
                            GameObject textObject = new GameObject("Canvass");
                            textObject.transform.SetParent(canvas.transform);

                            // Add Text component
                            displayText = textObject.AddComponent<Text>();

                            // Configure Text settings
                            displayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                            displayText.fontSize = 15;
                            displayText.color = Color.white; // Set text color to white
                            displayText.alignment = TextAnchor.MiddleCenter; // Center the text
                            displayText.horizontalOverflow = HorizontalWrapMode.Wrap; // Wrap text horizontally
                            displayText.verticalOverflow = VerticalWrapMode.Overflow; // Allow text to overflow vertically

                            // Add Outline component for the black border
                            Outline outline = textObject.AddComponent<Outline>();
                            outline.effectColor = Color.black; // Set border color to black
                            outline.effectDistance = new Vector2(1, -1); // Adjust for desired thickness

                            // Positioning and Scaling
                            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
                            rectTransform.localPosition = Vector3.zero; // Center of the screen
                            rectTransform.sizeDelta = new Vector2(300, 200); // Bigger area for text

                            // Start the coroutine to show text letter by letter
                            yield return StartCoroutine(DisplayTextLetterByLetter(content));
                    }
                    else
                    {
                        Debug.LogError("Response data or choices array is null/empty.");
                    }
            }
        }
    }

        private IEnumerator SendRequest2(string mytext)
    {
        cube1.SetActive(true);
        cube2.SetActive(true);
        cube3.SetActive(true);
        desk.SetActive(true);

                    // Prepare the request data
                        string prompt = @"Based on the following story, create a simple memory game. 
                1. Frame a question based on the story. 
                2. Provide three possible answer options. 
                3. Ask the player to interact with three cubes:
                - Choose the red cube for Option 1.
                - Choose the green cube for Option 2.
                - Choose the blue cube for Option 3.
                IMPORTANT to output the result as a JSON object with the exact following structure:
                {
                ""questionText"": ""<question>"",
                ""optionText"": ""<cube colors with the options text>"",
                ""answer"": ""<correct cube color>""
                 ""answerText"": ""<correct answer text>""
                }

                The story: " + mytext;

                        // Create message object
                        Message userMessage = new Message
                        {
                            role = "user",
                            content = prompt
                        };

                        // Create request data
                        RequestData requestData = new RequestData
                        {
                            messages = new List<Message> { userMessage },
                            model = "Llama-3.3-70b-Versatile"
                        };

                        // Serialize to JSON
                        string jsonData = JsonConvert.SerializeObject(requestData);
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(apiUrl, jsonData))
        {
            // Set request headers
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.SetRequestHeader("Authorization", "Bearer " + GetApiKey());
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Attach the JSON data
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;

             // Parse the response
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseText);

                    // Extract content from the response
                    if (responseData != null && responseData.choices != null && responseData.choices.Length > 0)
                    {
                        string content = responseData.choices[0].message.content;
                        Debug.Log($"ENEW: {content}");
                        string questionText = ExtractBetween(content, @"""questionText"": """, @""",");
                        string optionText = ExtractBetween(content, @"""optionText"": """, @"""");
                        string newcontent =  questionText +  " " + optionText; 
                        answer = ExtractBetween(content, @"""answer"": """, @"""");
                        answertext = ExtractBetween(content, @"""answerText"": """, @"""");
                        if(answer == "" || answertext == "")
                        {
                                SendRequest2(mytext);
                        }
                        else{
                            yield return StartCoroutine(DisplayTextLetterByLetter2(content, newcontent));
                        }

                    }
                    else
                    {
                        Debug.LogError("Response data or choices array is null/empty.");
                    }
            }
        }
    }
    // Helper method to extract text between two substrings
    private static string ExtractBetween(string source, string start, string end)
    {
        int startIndex = source.IndexOf(start) + start.Length;
        int endIndex = source.IndexOf(end, startIndex);
        return source.Substring(startIndex, endIndex - startIndex);
    }
 IEnumerator DisplayTextLetterByLetter(string text)
{
    displayText.text = ""; // Start with an empty string

    // Loop through each character in the string
    foreach (char letter in text)
    {
        displayText.text += letter; // Add one letter at a time
        yield return new WaitForSeconds(0.1f); // Wait 0.1 seconds before showing the next letter
    }

    // Wait for 10 seconds after the text is fully displayed
    yield return new WaitForSeconds(10f);

    // Remove the text object from the scene (or disable it)
    displayText.text = "";
      StartCoroutine(SendRequest2(text));
}
 IEnumerator DisplayTextLetterByLetter2(string oldtext2, string text)
{
    displayText.text = ""; // Start with an empty string

    // Loop through each character in the string
    foreach (char letter in text)
    {
        displayText.text += letter; // Add one letter at a time
        yield return new WaitForSeconds(0.1f); // Wait 0.1 seconds before showing the next letter
    }

    // Wait for 10 seconds after the text is fully displayed
    yield return new WaitForSeconds(5f);

    displayText.text = "";


}


    private string GetApiKey()
    {
	//your api key
        return ""; 
        
     }
     [System.Serializable]
public class ResponseData
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
    [Serializable]
    public class RequestData
    {
        public List<Message> messages;
        public string model;
    }
 
}
