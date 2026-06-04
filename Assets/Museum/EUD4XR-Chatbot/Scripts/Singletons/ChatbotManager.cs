using System;
using System.Collections;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using Serilog;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;


public class ChatbotManager : Singleton<ChatbotManager>
{
    public InputActionReference interactionButton;

    public bool isLogged = false;

    /* Look at MicrophoneMaanger.MonitorAudioLevel */
    private bool _isSpeaking = false;
    // private float lastSpeechTime;
    //private AudioClip microphoneClip;
    // private const int SampleRate = 44100;
    // private const float Threshold = 0.02f;
    // private const float isSpeakingTolerance = 1.5f;
    // check update
    // private float checkInterval = 0.5f;
    // private float lastSpeechCheckTime = 0f;


    private void OnEnable()
    {
        Debug.Log("Enabling ChatbotManager");
        StartCoroutine(ForceLogin(isFirstLogin: true));
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

                    ChatbotUIManager.Instance.FeedbackAutomationCreated(chatbotAnswer.currNode);
                }

                void IfFailedGeneratingVoice(string ttsErrorMessage)
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
                    // Update text but not audio since it crashed
                    // ChatbotUIManager.Instance.SpeakTranscription(botVoiceClip, AfterAudioPlaybackCompleted);
                    AfterAudioPlaybackCompleted();
                }

                // Update text
                ChatbotUIManager.Instance.UpdateTranscription("Ho la risposta pronta! Mi preparo a dirtela...");
                // Update animation
                ChatbotAnimationController.RequestAnimationChange(ChatbotState.PreparingAnswerAudio);
                // Call Listeners
                // EndedGeneratingAnswer?.Invoke();
                // The chatbot answered, generate the fake voice
                StartCoroutine(EndpointUtils_Post_GenerateAudio(chatbotAnswer.message, AfterFakeVoiceGenerated,
                    IfFailedGeneratingVoice));
            }

            // Update text
            ChatbotUIManager.Instance.UpdateTranscription("Sto preparando il primo messaggio...dammi qualche secondo");
            // Update animation
            ChatbotAnimationController.RequestAnimationChange(ChatbotState.GeneratingAnswer);
            // Call listeners
            // EndedAnalyzingUserInput?.Invoke();

            // The transcription is ready, ask the chatbot
            StartCoroutine(this.AskChatbot(defaultStartMessage, AfterChatbotAnswered)); //TODO 25-08-27 Part1
        }

        GetDefaultMessage(); //TODO 25-08-27 Part2 J - We may want to remove this because it's a double Ask 
    }

    public void HandleRecording(InputAction.CallbackContext context)
    {
        if (_isSpeaking)
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
        if (!_isSpeaking)
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
            _isSpeaking = true;
            // lastSpeechTime = Time.time;
        }
    }

    public void HandleEndRecording()
    {
        Debug.Log("Ending recording");

        _isSpeaking = false;

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
                            if (chatbotAnswer.currNode == "exportAgent")
                            {
                                StartCoroutine(ResetSession());
                            }
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

                    void IfFailedGeneratingVoice(string ttsErrorMessage)
                    {
                        void AfterAudioPlaybackCompleted()
                        {
                            // Update animation
                            ChatbotAnimationController.RequestAnimationChange(ChatbotState.Idle);
                            if (chatbotAnswer.currNode == "exportAgent")
                            {
                                StartCoroutine(ResetSession());
                            }
                        }

                        // Trigger the EndedGeneratingAudioAnswer event
                        // EndedGeneratingAudioAnswer?.Invoke();
                        // Update Text
                        ChatbotUIManager.Instance.UpdateTranscription(chatbotAnswer.message);
                        // Update animation
                        ChatbotAnimationController.RequestAnimationChange(ChatbotState.Answering);
                        // Update audio 
                        // ChatbotUIManager.Instance.SpeakTranscription(botVoiceClip, AfterAudioPlaybackCompleted);
                        AfterAudioPlaybackCompleted();
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
                    StartCoroutine(EndpointUtils_Post_GenerateAudio(chatbotAnswer.message, AfterFakeVoiceGenerated,
                        IfFailedGeneratingVoice));
                }

                // Update text
                ChatbotUIManager.Instance.UpdateTranscription(
                    "Ho analizzato ciò che hai detto, ora genero una risposta...dammi qualche secondo");
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
            StartCoroutine(EndpointUtils_Post_TranscribeAudio(clipBytes, AfterTranscriptionGenerated));
        }

        MicrophoneManager.Instance.EndRecording(AfterUserStoppedSpeaking);

        WakeUpWordHandler.Instance.picovoiceManager.Start();
    }

    private IEnumerator AskChatbot(string userMessage, System.Action<AIMessageResponse> callback)
    {
        yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);
        // Do a Unity POST request to the chatbot server with the { message = userMessage }
        // The server will respond with a JSON object { success: {true, false}, message: <string answer> }
        string urlChatAI = DualChatbotServerSettings.Instance.GetChatAIUrl(); 
        //string urlChatAI = DualChatbotServerSettings.Instance.GetFakeAnswerUrl(); // or if you want a fake answer
        
        using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(urlChatAI, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"message\": \"" + userMessage + "\"}");
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
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
                Log.Information($"Risposta ricevuta: {responseText}");
                var jsonResponse = JsonConvert.DeserializeObject<AIMessageResponse>(responseText);
                if (jsonResponse != null)
                {
                    Log.Information("Transcription: " + jsonResponse.message);
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

    private IEnumerator EndpointUtils_Post_TranscribeAudio(byte[] audioBytes, System.Action<string> onSuccessCallback)
    {
        yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);

        
        string url =   DualChatbotServerSettings.Instance.GetTranscriptionAudioUrl(); ;

        void MyCustomCallback(string jsonResponse)
        {
            // Optional: Parse the JSON response if needed (assuming it's in JSON format)
            try
            {
                // Example of parsing a JSON response
                var audioResponse = JsonConvert.DeserializeObject<TranscriptionResponse>(jsonResponse);
                if (audioResponse != null && audioResponse.success)
                {
                    Debug.Log("Transcription: " + audioResponse.transcription);
                    onSuccessCallback?.Invoke(audioResponse.transcription);
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

        yield return UnityWebRequestUtils.POST_RAWFILE_return_JSON(url, audioBytes , MyCustomCallback);
    }
    
    private static IEnumerator EndpointUtils_Post_GenerateAudio(string textToSpeak, System.Action<AudioClip> callback, System.Action<string> onErrorCallback)
    {
        yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);

        string url =   DualChatbotServerSettings.Instance.GetGenerateAudioUrl(); 

        
        // Create the JSON payload
        var jsonPayload = new { message = textToSpeak };
        string jsonString = JsonConvert.SerializeObject(jsonPayload);

        // Create the UnityWebRequest
        UnityWebRequest uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonString);

        // Use UploadHandlerRaw for sending the JSON payload
        uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG); // Expect MP3 data
        uwr.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
            // if (true)
        {
            Debug.LogError("Error in TTS Request: " + uwr.error);
            onErrorCallback?.Invoke(uwr.error);
        }
        else
        {
            Debug.Log("TTS Request Successful, Processing Audio...");

            // Directly get the AudioClip from the response
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            callback?.Invoke(audioClip);
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

    // Define a response structure to match your server's JSON response
    [System.Serializable]
    private class TranscriptionResponse
    {
        public bool success;
        public string transcription;
    }
    
    private IEnumerator ForceLogin(bool isFirstLogin)
    {
        yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);

        if (isLogged)
        {
            Debug.LogWarning("Already logged in.");
            yield break;
        }

        // Get request to the forceLoginUrl
        // The server will respond with a JSON object { success: {true, false} }
        string forceLoginUrl = DualChatbotServerSettings.Instance.GetForceLoginUrl();
        var url = forceLoginUrl + $"?firstLogin={isFirstLogin}";
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
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
                    Debug.Log("Forced login successful with url: " + url);
                    isLogged = true;
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
        if (!isLogged)
        {
            Debug.LogWarning("Already logged out.");
            yield break;
        }

        // Do a Post request to the forceLogoutUrl
        // The server will respond with a JSON object { success: {true, false} }
        string forceLogoutUrl = DualChatbotServerSettings.Instance.GetForceLogoutUrl();
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
                    isLogged = false;
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

    [ContextMenu("TEST RESET SESSION")]
    void TestCoroutineResetSession()
    {
        Debug.Log("[RESETSESSION] BEFORE STOPPING ALL COROUTINES");
        StopAllCoroutines();
        Debug.Log("[RESETSESSION] AFTER STOPPING ALL COROUTINES");
        StartCoroutine(ResetSession());
    }

    private IEnumerator ResetSession()
    {
        yield return ForceLogout();
        Debug.Log("[RESETSESSION] HO FATTO LOGOUT");
        yield return ForceLogin(isFirstLogin: false);
        Debug.Log("[RESETSESSION] HO FATTO LOGIN");

        const string defaultStartMessage = "Ciao";

        void GetDefaultMessage()
        {
            // The transcription is ready, ask the chatbot
            StartCoroutine(this.AskChatbot(defaultStartMessage, null));
        }

        GetDefaultMessage();
    }
}