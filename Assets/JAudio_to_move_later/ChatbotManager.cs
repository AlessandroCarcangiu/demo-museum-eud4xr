using UnityEngine;

public class ChatbotManager : Singleton<ChatbotManager>
{
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
                void OnUserStoppedSpeaking(AudioClip clip, byte[] clipBytes)
                {
                    void AfterTranscriptionGenerated(string transcription)
                    {
                        ChatbotUIManager.Instance.UpdateTranscription(transcription);
                        ChatbotUIManager.Instance.SpeakTranscription(clip);
                    }
                    ChatbotUIManager.Instance.UpdateTranscription("Transcribing...");

                    // The user stopped speaking, the clip contains the audio
                    StartCoroutine(Speech2Text.Transcribe(clipBytes, AfterTranscriptionGenerated));
                }

                MicrophoneManager.Instance.EndRecording(OnUserStoppedSpeaking);
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
}