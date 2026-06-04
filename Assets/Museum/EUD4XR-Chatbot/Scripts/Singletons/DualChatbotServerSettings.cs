using System;
using ECARules4All_DLL.Utils;
using UnityEngine;

[System.Serializable]
public class HASS_Settings
{
    public string url;
    public string token;
    public bool doLog;
}

[System.Serializable]
public class SEQ_Settings
{
    public string url;
    public string apiKey;
}

[System.Serializable]
public class Marker_Settings
{
    public string urlJson;
}

// [DefaultExecutionOrder(-100)]
public class DualChatbotServerSettings : Singleton<DualChatbotServerSettings>
{
    ///////////////////////// CHANGE THESE URLS TO POINT TO YOUR SERVERS //////////////////////
    private const string URL_SERVER_CAGLIARI = "http://localhost:3000/";
    // private const string URL_SERVER_CAGLIARI = "http://mercurio.isti.cnr.it:3000/";
    private const string URL_SERVER_PISA = "https://urlPisa:3000/";

    private const string ENDPOINT_GET_ALL_SETTINGS = "api/get-all-settings";
    ///////////////////////////////////////////////////////////////////////////////////////////

    // //region Public event
    // /// <summary>
    // /// Fired once when settings are initialized from the first responding server.
    // /// Args: (selectedServerUrl, hass, seq, markers)
    // /// </summary>
    // public event Action<string, HASS_Settings, SEQ_Settings, Marker_Settings> SettingsLoaded;
    // //endregion
    
    /// <summary>
    /// Returns true after settings have been initialized from any server.
    /// </summary>
    public bool IsInitialized => !string.IsNullOrEmpty(_urlServerToUse) && allSettings != null;
        
    //region Private Internal Stuff
    private string _urlServerToUse = string.Empty;

    [System.Serializable]
    private class AllSettings
    {
        public SEQ_Settings seq;
        public HASS_Settings hass;
        public Marker_Settings markers;
    }

    private AllSettings allSettings;

    private void OnGetSettingFromWeb(string serverUrl, string json)
    {
        if (!string.IsNullOrEmpty(_urlServerToUse)) // if already initialized, error
        {
            throw new Exception("Something went wrong. Double initialization of settings");
        }

        if (string.IsNullOrEmpty(serverUrl)) // if I didn't pass a server URL, error
        {
            throw new Exception("Something went wrong. No location specified");
        }


        try
        {
            var s = JsonUtility.FromJson<AllSettings>(json);
            allSettings = s;
            // Debug.unityLogger.logEnabled = allSettings.doLog;
            _urlServerToUse = serverUrl;
            Debug.Log("New _urlServerToUse: " + _urlServerToUse);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Non potevo perché e: " + e);
        }
    }

    protected override void OnAwake()
    {
        const string CAGLIARI_GET_SETTINGS_URL = URL_SERVER_CAGLIARI + ENDPOINT_GET_ALL_SETTINGS;
        const string PISA_GET_SETTINGS_URL = URL_SERVER_PISA + ENDPOINT_GET_ALL_SETTINGS;


        StartCoroutine(
            UnityWebRequestUtils.GET_JSON(
                CAGLIARI_GET_SETTINGS_URL,
                json => OnGetSettingFromWeb(URL_SERVER_CAGLIARI, json)
            )
        );

        StartCoroutine(
            UnityWebRequestUtils.GET_JSON(
                PISA_GET_SETTINGS_URL,
                json => OnGetSettingFromWeb(URL_SERVER_PISA, json)
            ));
    }
    //endregion


    //region Chatbot Endpoints
    public string GetChatAIUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "api/message";
    }

    public string GetFakeAnswerUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "api/fake-answer";
    }

    public string GetForceLoginUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "force-login-admin";
    }

    public string GetForceLogoutUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "api/logout";
    }
    
    public string GetTranscriptionAudioUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "api/audio";
    }
    
    public string GetGenerateAudioUrl()
    {
        if (string.IsNullOrEmpty(_urlServerToUse))
        {
            throw new Exception("Server URL not initialized yet");
        }

        return _urlServerToUse + "api/generate-speech";
    }
    //endregion

    //region Settings Getters
    public HASS_Settings GetSettings_HASS()
    {
        if (allSettings == null)
        {
            throw new Exception("Settings not initialized yet");
        }

        return allSettings.hass;
    }
    
    public SEQ_Settings GetSettings_SEQ()
        {
            if (allSettings == null)
            {
                throw new Exception("Settings not initialized yet");
            }
    
            return allSettings.seq;
        }

    public Marker_Settings GetSettings_Markers()
    {
        if (allSettings == null)
        {
            throw new Exception("Settings not initialized yet");
        }

        return allSettings.markers;
    }

//endregion
}