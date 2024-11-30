using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ChatbotManager : Singleton<ChatbotManager>
{
    const string urlChatAI = "http://localhost:3000/api/fake-answer";
    private bool UserPressedInteractionButton()
    {
        return Input.GetKeyDown(KeyCode.Space);
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
                var jsonResponse = JsonConvert.DeserializeObject<AIResponse>(responseText);
                if (jsonResponse != null && jsonResponse.success)
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
    private class AIResponse
    {
        public bool success;
        public string message;
    }
}