using System.Collections;
using System.Collections.Generic;
using System.IO;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;
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
        
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = settings.hassUrl;
        //hassClient.url = "https://fly-powerful-slug.ngrok-free.app";//
        //hassClient.token =
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI5YjI0M2YzNmY2NTk0MmEyOTM4NGNmODk0MjZhNzQxZCIsImlhdCI6MTcyNTk3NTI2MSwiZXhwIjoyMDQxMzM1MjYxfQ.ylQDAWp0lEs1OGgjxGxAO7LYavuon-FuorspQhM8kDI";
        hassClient.token = settings.hassToken;
        RuleEngine.GetInstance().AddClient(hassClient);
        _apiServer = new APIServer();
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            string filePath = "Assets/Scripts/automationsV2.json";
            string jsonContent = File.ReadAllText(filePath);
            List<AutomationDTO> rules = JsonConvert.DeserializeObject<List<AutomationDTO>>(jsonContent);
            foreach (var rule in rules)
            {
	            Debug.Log($"Regola ricevuta: {rule.ToString()} - {rule.id}");
                Rule a = rule.ConvertToRule();
                Debug.Log($"Regola creata: {a.GetEvent()}");
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
