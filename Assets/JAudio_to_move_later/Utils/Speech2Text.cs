using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class Speech2Text
{
    const string url = "http://localhost:3000/api/audio";

    private static IEnumerator Transcribe(AudioClip clip, System.Action<string> callback)
    {
        // Convert the AudioClip to a byte array
        byte[] byteArray = SaveWav.Clip2Bytes(clip); //TODO Method NOT tested
        yield return Transcribe(byteArray, callback);
    }
    
    
    public static IEnumerator Transcribe(byte[] byteArray, System.Action<string> callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, "POST");
        
        // Set up the request with a byte array of raw data
        UploadHandler uploader = new UploadHandlerRaw(byteArray);
        uploader.contentType = "application/octet-stream"; // Set content type for raw binary data
        uwr.uploadHandler = uploader;
        
        // Set up the download handler to capture the response text
        DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
        uwr.downloadHandler = downloadHandler;
        
        yield return uwr.SendWebRequest();
        
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            throw new Exception("Error: " + uwr.error);
        }
        // Log the response text (the transcription result)
        string responseText = downloadHandler.text;
        Debug.Log("Response Text: " + responseText);

        // Optional: Parse the JSON response if needed (assuming it's in JSON format)
        try
        {
            // Example of parsing a JSON response
            var jsonResponse = JsonConvert.DeserializeObject<AudioResponse>(responseText);
            if (jsonResponse != null && jsonResponse.success)
            {
                Debug.Log("Transcription: " + jsonResponse.transcription);
                callback?.Invoke(jsonResponse.transcription);
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

    
    // Define a response structure to match your server's JSON response
    [System.Serializable]
    private class AudioResponse
    {
        public bool success;
        public string transcription;
    }

}