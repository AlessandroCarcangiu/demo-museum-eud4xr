using System;
using System.Collections;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;


public class ChatbotManager : Singleton<ChatbotManager>
{
    public InputActionReference interactionButton;

    private const string urlChatAI = "http://localhost:3000/api/message"; // "http://localhost:3000/api/fake-answer";
    //private const string urlChatAI = "http://localhost:3000/api/fake-answer"; 
    private const string forceLoginUrl = "http://localhost:3000/force-login-admin";
    private const string forceLogoutUrl = "http://localhost:3000/api/logout";
    
    /* Speech detection every 1 second */
    private bool isSpeaking = false;
    private float lastSpeechTime;
    //private AudioClip microphoneClip;
    private const int SampleRate = 44100;
    private const float Threshold = 0.02f;
    private const float isSpeakingTolerance = 1.5f;
    // check update
    private float checkInterval = 0.5f;
    private float lastSpeechCheckTime = 0f;
    
    private bool UserPressedInteractionButton()
    {
        return interactionButton.action.triggered;
    }
    
    private void OnEnable()
    {
        Debug.Log("Enabling ChatbotManager");
        StartCoroutine(ForceLogin());
        interactionButton.action.performed += HandleRecording;
    }

        private void Start()
    {
        // Checks?

        const string defaultStartMessage = "Ciao";
        void GetDefaultMessage()
        {
            void AfterChatbotAnswered(AIMessageResponse chatbotAnswer)
            {
                void AfterFakeVoiceGenerated(AudioClip botVoiceClip)
                {
                    void AfterAudioPlaybackCompleted()
                    {
                        // Update animation
                        ChatbotAnimationController.RequestAnimationChange(ChatbotState.Idle);
                    }

                    // Trigger the EndedGeneratingAudioAnswer event
                    // EndedGeneratingAudioAnswer?.Invoke();
                    // Update Text
                    ChatbotUIManager.Instance.UpdateTranscription(chatbotAnswer.message);
                    // Update animation
                    ChatbotAnimationController.RequestAnimationChange(ChatbotState.Answering);
                    // Update audio 
                    ChatbotUIManager.Instance.SpeakTranscription(botVoiceClip, AfterAudioPlaybackCompleted);
                }

                // Update text
                ChatbotUIManager.Instance.UpdateTranscription("Ho la risposta pronta! Mi preparo a dirtela...");
                // Update animation
                ChatbotAnimationController.RequestAnimationChange(ChatbotState.PreparingAnswerAudio);
                // Call Listeners
                // EndedGeneratingAnswer?.Invoke();
                // The chatbot answered, generate the fake voice
                StartCoroutine(Text2Speech.CreateAudio(chatbotAnswer.message, AfterFakeVoiceGenerated));
            }

            // Update text
            ChatbotUIManager.Instance.UpdateTranscription("Sto preparando il primo messaggio...dammi qualche secondo");
            // Update animation
            ChatbotAnimationController.RequestAnimationChange(ChatbotState.GeneratingAnswer);
            // Call listeners
            // EndedAnalyzingUserInput?.Invoke();

            // The transcription is ready, ask the chatbot
            StartCoroutine(this.AskChatbot(defaultStartMessage, AfterChatbotAnswered));
        }

        GetDefaultMessage();
    }
        
    public void HandleRecording(InputAction.CallbackContext context)
    {
        if (isSpeaking)
        {
            HandleEndRecording();
        }
        else
        {
            HandleStartRecording();
        }
    }
    
    public void HandleStartRecording()
    {
        if(!isSpeaking)
        {
            WakeUpWordHandler.Instance.picovoiceManager.Stop();
            
            Debug.Log("Started recording");

            // Start the recording
            MicrophoneManager.Instance.StartRecording();
            
            // Stop the chatbot from speaking
            ChatbotUIManager.Instance.StopSpeaking();
            
            // Update Text
            ChatbotUIManager.Instance.UpdateTranscription("Ti sto ascoltando :)");
            // Update Animation
            ChatbotAnimationController.RequestAnimationChange(ChatbotState.Listening);

            // Start speaking
            isSpeaking = true;
            lastSpeechTime = Time.time;
        }
    }
    
    public void HandleEndRecording()
    {
        Debug.Log("Ending recording");
        
        isSpeaking = false;
        
        // End the recording
        void AfterUserStoppedSpeaking(AudioClip clip, byte[] clipBytes)
        {
            void AfterTranscriptionGenerated(string transcription)
            {
                void AfterChatbotAnswered(AIMessageResponse chatbotAnswer)
                {
                    void AfterFakeVoiceGenerated(AudioClip botVoiceClip)
                    {
                        void AfterAudioPlaybackCompleted()
                        {
                            // Update animation
                            ChatbotAnimationController.RequestAnimationChange(ChatbotState.Idle);
                        }
                        
                        // Trigger the EndedGeneratingAudioAnswer event
                        // EndedGeneratingAudioAnswer?.Invoke();
                        // Update Text
                        ChatbotUIManager.Instance.UpdateTranscription(chatbotAnswer.message);
                        // Update animation
                        ChatbotAnimationController.RequestAnimationChange(ChatbotState.Answering);
                        // Update audio 
                        ChatbotUIManager.Instance.SpeakTranscription(botVoiceClip, AfterAudioPlaybackCompleted);
                        // Make feedback if an automation is triggered
                        ChatbotUIManager.Instance.FeedbackAutomationCreated(chatbotAnswer.currNode);
                    }

                    // Update text
                    ChatbotUIManager.Instance.UpdateTranscription("Ho la risposta pronta! Mi preparo a dirtela...");
                    // Update animation
                    ChatbotAnimationController.RequestAnimationChange(ChatbotState.PreparingAnswerAudio);
                    // Call Listeners
                    // EndedGeneratingAnswer?.Invoke();
                    // The chatbot answered, generate the fake voice
                    StartCoroutine(Text2Speech.CreateAudio(chatbotAnswer.message, AfterFakeVoiceGenerated));
                }
                
                // Update text
                ChatbotUIManager.Instance.UpdateTranscription("Ho analizzato ciò che hai detto, ora genero una risposta...dammi qualche secondo");
                // Update animation
                ChatbotAnimationController.RequestAnimationChange(ChatbotState.GeneratingAnswer);
                // Call listeners
                // EndedAnalyzingUserInput?.Invoke();
                
                // The transcription is ready, ask the chatbot
                StartCoroutine(this.AskChatbot(transcription, AfterChatbotAnswered));
            }
            
            // Update the transcription UI
            ChatbotUIManager.Instance.UpdateTranscription("Sto analizzando ciò che hai detto...dammi qualche secondo");
            // Update the Avatar animation
            ChatbotAnimationController.RequestAnimationChange(ChatbotState.Analyzing);
            // If there are listeners, trigger the EndedListeningUser event
            // EndedListeningUser?.Invoke();
            
            // The user stopped speaking, the clip contains the audio
            StartCoroutine(Speech2Text.Transcribe(clipBytes, AfterTranscriptionGenerated));
        }
        
        MicrophoneManager.Instance.EndRecording(AfterUserStoppedSpeaking);
        
        WakeUpWordHandler.Instance.picovoiceManager.Start();
    }
    
    private IEnumerator AskChatbot(string userMessage, System.Action<AIMessageResponse> callback)
    {
        // Do a Unity POST request to the chatbot server with the { message = userMessage }
        // The server will respond with a JSON object { success: {true, false}, message: <string answer> }
        
        using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(urlChatAI, "POST"))
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
                    callback?.Invoke(jsonResponse);
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
        using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(forceLogoutUrl, "POST"))
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