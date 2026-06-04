using System.Collections;
using System.Collections.Generic;
using System.IO;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Networking;

public class HomeAssistantUnityIntegration : Singleton<HomeAssistantUnityIntegration>
{
    [System.Serializable]
    public class Settings
    {
        public string hassUrl;
        public string hassToken;
        public bool doLog;
    }
    
    private APIServer _apiServer;

    public Settings settings;
    
    private string path;

    private bool isStarted = false;

    private void Awake()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            settings = JsonUtility.FromJson<Settings>(json);
            Debug.Log("Secrets loaded successfully.");
        }
        else
        {
            Debug.LogError($"Secrets file not found at path: {path}");
        }
        
        //string path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        //path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        //StartCoroutine(ReadSettings());
        Debug.unityLogger.logEnabled = settings.doLog;
        
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        if (settings != null && !string.IsNullOrEmpty(settings.hassUrl)) {
            hassClient.url = settings.hassUrl;
            hassClient.token = settings.hassToken;
        } else {
            Debug.LogError("ATTENZIONE: settings.hassUrl è vuoto o nullo! Controlla l'Inspector.");
        }
        Debug.Log($"url: {hassClient.url}");
        
        RuleEngine.GetInstance().AddClient(hassClient);
        _apiServer = new APIServer(8080);
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
    }

    IEnumerator ReadSettings()
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Errore nella lettura del file: " + request.error);
        }
        else
        {
            string jsonString = request.downloadHandler.text;
            Debug.Log("File letto correttamente: " + jsonString);
            settings = JsonUtility.FromJson<Settings>(jsonString);
            Debug.Log($"hassUrl: {settings.hassUrl}, hassToken: {settings.hassToken}");
        }
    }


    // Start is called before the first frame update
    void Start()
    { 
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app" - unity
        // ngrok http 8123 --host-header="localhost:8123" --domain="fly-powerful-slug.ngrok-free.app" - hass 
        
        
        
        //rule QuadroOphelia
        GameObject quadro = GameObject.Find("Quadro_Ophelia");
        GameObject interactor = GameObject.Find("Player");
        GameObject spotlight = GameObject.Find("Faretto_Ophelia");

        if (quadro != null && interactor != null && spotlight != null)
        {
            Action trigger = new Action(interactor, "interacts with", quadro);
            Action action = new Action(spotlight, "turns", ECABoolean.ON);
            
            RuleEngine.GetInstance().Add(new Rule(trigger, new List<Action> { action }));
            UnityEngine.Debug.Log("<color=green><b>[REGOLA] Avvicinamento a Ophelia configurato!</b></color>");
        }
        else
        {
            UnityEngine.Debug.LogError("[ECA SETUP] Errore: Controlla i nomi di Quadro, Interactor o Faretto nella Hierarchy!");
        }
    }

    void Update()
    {
        InputDevice rightHandController = null;
        foreach (var device in InputSystem.devices)
        {
            if (device.usages.Contains(CommonUsages.RightHand))
            {
                rightHandController = device;
                break;
            }
        }

        if (rightHandController != null)
        {
            var aButton = rightHandController["primaryButton"] as UnityEngine.InputSystem.Controls.ButtonControl;
            if (aButton == null)
            {
                aButton = rightHandController["buttonSouth"] as UnityEngine.InputSystem.Controls.ButtonControl;
            }

            if (aButton != null && aButton.wasPressedThisFrame)
            {
                GameObject spotlight = GameObject.Find("Faretto_Ophelia");
                if (spotlight != null)
                {
                    Action testAction = new Action(spotlight, "turns", ECABoolean.ON);
                    RuleEngine.GetInstance().ExecuteAction(testAction);
                    UnityEngine.Debug.Log("<color=lime><b>[AZIONE] Tasto A premuto: Comando Faretto ON inviato!</b></color>");
                }
            }
        }
    }
    
    
    private void OnDisable()
    {
        if (isStarted)
        {
            CloseServer();
            isStarted = false;
        }
    }

    private void OnDestroy()
    {
        CloseServer();
    }

    private void CloseServer()
    {
        if(_apiServer != null)
        {
            _apiServer.Stop();
        }
    }
}
