using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestUtils
{
    /**
     * GET request that returns only the response text in the callback.
     */
    public static IEnumerator GET_JSON(string urlGET, Action<string> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess), "Callback cannot be null");

        Debug.Log("DDD Doing GET JSON from path: " + urlGET);
        // Build the path to the JSON file in StreamingAssets

        if (urlGET.StartsWith("http"))
        {
            Debug.Log("DDD 1 File starts with http");

            // Special case to access StreamingAsset content on Android and Web
            using (UnityWebRequest uwr = UnityWebRequest.Get(urlGET))
            {
                Debug.Log("DDD Before SendWebRequest");

                yield return uwr.SendWebRequest();
                Debug.Log("DDD After SendWebRequest");

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"DDD Url:{urlGET} | Error: {uwr.error}");
                }

                string responseText = uwr.downloadHandler.text;
                Debug.Log(
                    $"DDD Successfully loaded JSON file from {urlGET} with content {responseText}. Now calling callback.");
                onSuccess.Invoke(responseText);
            }
        }
        else
        {
            throw new NotSupportedException("DDD Only HTTP(S) paths are supported in this version.");
        }
    }

    public static IEnumerator GET_JSON(string urlGET, string bearerToken, Action<string> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess), "Callback cannot be null");

        Debug.Log("[GET_JSON] Doing GET JSON from path: " + urlGET);
        // Build the path to the JSON file in StreamingAssets

        if (urlGET.StartsWith("http"))
        {
            Debug.Log("[GET_JSON] 1 File starts with http");

            // Special case to access StreamingAsset content on Android and Web
            using (UnityWebRequest uwr = UnityWebRequest.Get(urlGET))
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + bearerToken);
                Debug.Log("[GET_JSON] Before SendWebRequest");

                yield return uwr.SendWebRequest();
                Debug.Log("[GET_JSON] After SendWebRequest");

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"[GET_JSON] Url:{urlGET} | Error: {uwr.error}");
                }

                string responseText = uwr.downloadHandler.text;
                Debug.Log(
                    $"[GET_JSON] Successfully loaded JSON file from {urlGET} with content {responseText}. Now calling callback.");
                onSuccess.Invoke(responseText);
            }
        }
        else
        {
            throw new NotSupportedException("[GET_JSON] Only HTTP(S) paths are supported in this version.");
        }
    }


    public static IEnumerator POST_RAWFILE_return_JSON(string urlPost, byte[] fileBytes, Action<string> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess), "Callback cannot be null");

        Debug.Log("[POST_RAWFILE_return_JSON] Doing POST Http Url: " + urlPost);

        if (urlPost.StartsWith("http"))
        {
            UnityWebRequest uwr = UnityWebRequest.PostWwwForm(urlPost, "POST");

            // Set up the request with a byte array of raw data
            UploadHandler uploader = new UploadHandlerRaw(fileBytes);
            uploader.contentType = "application/octet-stream"; // Set content type for raw binary data
            uwr.uploadHandler = uploader;

            // Set up the download handler to capture the response text
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
            uwr.downloadHandler = downloadHandler;

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"[POST_RAWFILE_return_JSON] Url:{urlPost} | Error: {uwr.error}");
            }

            // Log the response text (the transcription result)
            string responseText = downloadHandler.text;
            Debug.Log(
                $"[POST_RAWFILE_return_JSON] Successfully loaded JSON file from {urlPost} with content {responseText}. Now calling callback.");
            onSuccess.Invoke(responseText);
        }
        else
        {
            throw new NotSupportedException("DDD Only HTTP(S) paths are supported in this version.");
        }
    }
    
    public static IEnumerator POST_RAWFILE_return_JSON(string urlPost, byte[] fileBytes, string bearerToken, Action<string> onSuccess)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess), "Callback cannot be null");

        Debug.Log("[POST_RAWFILE_return_JSON] Doing POST Http Url: " + urlPost);

        if (urlPost.StartsWith("http"))
        {
            UnityWebRequest uwr = UnityWebRequest.PostWwwForm(urlPost, "POST");

            uwr.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            
            // Set up the request with a byte array of raw data
            UploadHandler uploader = new UploadHandlerRaw(fileBytes);
            uploader.contentType = "application/octet-stream"; // Set content type for raw binary data
            uwr.uploadHandler = uploader;

            // Set up the download handler to capture the response text
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
            uwr.downloadHandler = downloadHandler;

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"[POST_RAWFILE_return_JSON] Url:{urlPost} | Error: {uwr.error}");
            }

            // Log the response text (the transcription result)
            string responseText = downloadHandler.text;
            Debug.Log(
                $"[POST_RAWFILE_return_JSON] Successfully loaded JSON file from {urlPost} with content {responseText}. Now calling callback.");
            onSuccess.Invoke(responseText);
        }
        else
        {
            throw new NotSupportedException("DDD Only HTTP(S) paths are supported in this version.");
        }
    }
}