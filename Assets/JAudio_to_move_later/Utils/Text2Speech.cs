using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class Text2Speech
{
    const string url = "http://localhost:3000/api/generate-speech";

    // Usage: StartCoroutine(CreateAudio(textToSpeak, YourLogicHere));
    public static IEnumerator CreateAudio(string textToSpeak, System.Action<AudioClip> callback)
    {
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
        {
            Debug.LogError("Error in TTS Request: " + uwr.error);
        }
        else
        {
            Debug.Log("TTS Request Successful, Processing Audio...");

            // Directly get the AudioClip from the response
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            callback?.Invoke(audioClip);
            //TODO Delete
            // if (audioSource != null)
            // {
            //     audioSource.clip = audioClip;
            //     audioSource.Play();
            // }
            // else
            // {
                // Debug.LogError("AudioSource is not set!");
            // }
        }
    }
}