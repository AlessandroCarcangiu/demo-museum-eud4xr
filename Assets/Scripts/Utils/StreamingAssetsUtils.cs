using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class StreamingAssetsUtils
{
    public static IEnumerator GET_File(string streamingAssetsJsonPath, Action<string> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback), "Callback cannot be null");

        Debug.Log("[StreamingAssetsUtils - GET_FILE] Loading JSON file from StreamingAssets with name: " + streamingAssetsJsonPath);
        // Build the path to the JSON file in StreamingAssets
        string filePath = Path.Combine(Application.streamingAssetsPath, streamingAssetsJsonPath);

#if UNITY_EDITOR && !UNITY_ANDROID
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new Exception("CCC Cannot find JSON file at path: " + filePath);
            yield break;
        }
#endif

        string jsonData = "";
        if (filePath.StartsWith("jar") || filePath.StartsWith("http"))
        {
            // Special case to access StreamingAsset content on Android and Web
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                jsonData = request.downloadHandler.text;
                Debug.Log($"[StreamingAssetsUtils - GET_FILE] Successfully loaded JSON file from {filePath} with content {jsonData}");
                Debug.Log("[StreamingAssetsUtils - GET_FILE] Now calling callback.");
                callback.Invoke(jsonData);
            }
            else
            {
                throw new Exception($"[StreamingAssetsUtils - GET_FILE] Failed to load JSON file from {filePath}. \nError:" + request.error);
            }
        }
        else
        {
            // Read the file directly from the file system
            jsonData = File.ReadAllText(filePath);
            Debug.Log($"[StreamingAssetsUtils - GET_FILE] Successfully loaded JSON file from {filePath} with content {jsonData}");
            Debug.Log("[StreamingAssetsUtils - GET_FILE] Now calling callback.");
            callback.Invoke(jsonData);
        }
    }
}