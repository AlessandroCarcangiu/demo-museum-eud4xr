using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class ChatbotManager : Singleton<ChatbotManager>
{
    public InputActionReference interactionButton;

    private const string urlChatAI = "http://localhost:3000/api/message"; // "http://localhost:3000/api/fake-answer";
    private const string forceLoginUrl = "http://localhost:3000/force-login-admin";
    private const string forceLogoutUrl = "http://localhost:3000/api/logout";
    
    private bool UserPressedInteractionButton()
    {
        return interactionButton.action.triggered;
    }
    
    private void OnEnable()
    {
        Debug.Log("Enabling ChatbotManager");
        StartCoroutine(ForceLogin());
    }

    private void Update()
    {
        if (UserPressedInteractionButton())
        {
            Debug.Log("User pressed interaction button");

            // The user either started or stopped speaking, check the microphone manager
            if (MicrophoneManager.Instance.IsRecording())
            {
                Debug.Log("Ending recording");

                // End the recording
                void AfterUserStoppedSpeaking(AudioClip clip, byte[] clipBytes)
                {
                    void AfterTranscriptionGenerated(string transcription)
                    {
                        void AfterChatbotAnswered(string chatbotAnswer)
                        {
                            void AfterFakeVoiceGenerated(AudioClip botVoiceClip)
                            {
                                ChatbotUIManager.Instance.UpdateTranscription(chatbotAnswer);
                                ChatbotUIManager.Instance.SpeakTranscription(botVoiceClip);
                            }
                            
                            // The chatbot answered, generate the fake voice
                            StartCoroutine(Text2Speech.CreateAudio(chatbotAnswer, AfterFakeVoiceGenerated));
                        }

                        // The transcription is ready, ask the chatbot
                        StartCoroutine(this.AskChatbot(transcription, AfterChatbotAnswered));
                    }
                    
                    ChatbotUIManager.Instance.UpdateTranscription("Transcribing...");
                    // The user stopped speaking, the clip contains the audio
                    StartCoroutine(Speech2Text.Transcribe(clipBytes, AfterTranscriptionGenerated));
                }

                MicrophoneManager.Instance.EndRecording(AfterUserStoppedSpeaking);
            }
            else
            {
                Debug.Log("Started recording");

                // Start the recording
                MicrophoneManager.Instance.StartRecording();
                ChatbotUIManager.Instance.UpdateTranscription("Listening...");
            }
        }
    }
    
    private IEnumerator AskChatbot(string userMessage, System.Action<string> callback)
    {
        // Do a Unity POST request to the chatbot server with the { message = userMessage }
        // The server will respond with a JSON object { success: {true, false}, message: <string answer> }
        
        using (UnityWebRequest uwr = UnityWebRequest.Post(urlChatAI, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"message\": \"" + userMessage + "\"}");
            uwr.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + uwr.error);
                throw new System.Exception("Error: " + uwr.error);
            }
            
            string responseText = uwr.downloadHandler.text;

            try
            {
                // Example of parsing a JSON response
                var jsonResponse = JsonConvert.DeserializeObject<AIMessageResponse>(responseText);
                if (jsonResponse != null)
                {
                    Debug.Log("Transcription: " + jsonResponse.message);
                    callback?.Invoke(jsonResponse.message);
                }
                else
                {
                    throw new Exception("Transcription not found or failed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing JSON response: " + ex.Message);
            }
        }
    }
    
    // Define a response structure to match your server's JSON response
    [System.Serializable]
    private class AIMessageResponse
    {
        public string message;
        public string currNode;
        public string sessionId;
    }
    [System.Serializable]
    private class AILoginResponse
    {
        public bool success;
    }
    [System.Serializable]
    private class AILogoutResponse
    {
        public bool success;
    }
    

    private IEnumerator ForceLogin()
    {
        // Get request to the forceLoginUrl
        // The server will respond with a JSON object { success: {true, false} }
        
        using (UnityWebRequest uwr = UnityWebRequest.Get(forceLoginUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + uwr.error);
                throw new Exception("Error: " + uwr.error);
            }
            
            string responseText = uwr.downloadHandler.text;

            try
            {
                // Example of parsing a JSON response
                var jsonResponse = JsonConvert.DeserializeObject<AILoginResponse>(responseText);
                if (jsonResponse != null && jsonResponse.success)
                {
                    Debug.Log("Forced login successful.");
                }
                else
                {
                    throw new Exception("Forced login failed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing JSON response: " + ex.Message);
            }
        }
    }

    private IEnumerator ForceLogout()
    {
        // Do a Post request to the forceLogoutUrl
        // The server will respond with a JSON object { success: {true, false} }
        using (UnityWebRequest uwr = UnityWebRequest.Post(forceLogoutUrl, "POST"))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + uwr.error);
                throw new Exception("Error: " + uwr.error);
            }
            
            string responseText = uwr.downloadHandler.text;

            try
            {
                // Example of parsing a JSON response
                var jsonResponse = JsonConvert.DeserializeObject<AILogoutResponse>(responseText);
                if (jsonResponse != null && jsonResponse.success)
                {
                    Debug.Log("Forced logout successful.");
                }
                else
                {
                    throw new Exception("Forced logout failed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing JSON response: " + ex.Message);
            }
        }
        
    }

    private void OnDisable()
    {
        Debug.Log("Disabling ChatbotManager");
        StartCoroutine(ForceLogout());
    }
}