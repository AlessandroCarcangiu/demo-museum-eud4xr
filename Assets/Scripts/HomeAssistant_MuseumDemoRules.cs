using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
//using Newtonsoft.Json;
using Serilog;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Action = ECARules4All_DLL.Action;
using Path = ECARules4All_DLL.Path;
//using System.Text.Json;

//using Path = System.IO.Path;


public class HomeAssistant_MuseumDemoRules : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public string hassUrl;
        public string hassToken;
    }
    
    private APIServer _apiServer;

    private Settings _settings;
    
    private string path;

    private bool isStarted = false;

    private void Awake()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings_old.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _settings = JsonUtility.FromJson<Settings>(json);
            Debug.Log("Secrets loaded successfully.");
        }
        else
        {
            Debug.LogError($"Secrets file not found at path: {path}");
        }
        
        //string path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        //path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        //StartCoroutine(ReadSettings());
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
            _settings = JsonUtility.FromJson<Settings>(jsonString);
            Debug.Log($"hassUrl: {_settings.hassUrl}, hassToken: {_settings.hassToken}");
        }
    }


    // Start is called before the first frame update
    void Start()
    { 
        // Home Assistant configuration
        // configure home assistant client
        /*try
        {
            AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
            Debug.Log("1");
            hassClient.url = "https://fly-powerful-slug.ngrok-free.app";//_settings.hassUrl;
            Debug.Log("2");
            hassClient.token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI5YjI0M2YzNmY2NTk0MmEyOTM4NGNmODk0MjZhNzQxZCIsImlhdCI6MTcyNTk3NTI2MSwiZXhwIjoyMDQxMzM1MjYxfQ.ylQDAWp0lEs1OGgjxGxAO7LYavuon-FuorspQhM8kDI";//_settings.hassToken;
            Debug.Log("3");
            RuleEngine.GetInstance().AddClient(hassClient);
            Debug.Log("4");
            
            // configure api server
            //_apiServer = new APIServer();
            //_apiServer = new APIServer("http://0.0.0.0", 5000);//"192.168.1.123");
            //_apiServer = new APIServer("http://192.168.1.123", 5000);
            _apiServer = new APIServer("http://+", 5000);
            Debug.Log("5");
            _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
            Debug.Log("6");
        }
        catch (Exception e)
        {
            Debug.Log($"ERRORE NELLO START DI HOMEASSISTANTDEMORULES {e.Message}");
        }*/
        
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app" - unity
        // ngrok http 8123 --host-header="localhost:8123" --domain="fly-powerful-slug.ngrok-free.app" - hass 
        
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = _settings.hassUrl;
        //hassClient.url = "https://fly-powerful-slug.ngrok-free.app";//
        //hassClient.token =
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI5YjI0M2YzNmY2NTk0MmEyOTM4NGNmODk0MjZhNzQxZCIsImlhdCI6MTcyNTk3NTI2MSwiZXhwIjoyMDQxMzM1MjYxfQ.ylQDAWp0lEs1OGgjxGxAO7LYavuon-FuorspQhM8kDI";
        hassClient.token = _settings.hassToken;
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

    private void StartServer()
    {
        try
        {
            AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
            hassClient.url = _settings.hassUrl;
            //hassClient.url = "https://fly-powerful-slug.ngrok-free.app";//
            //hassClient.token =
            //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI5YjI0M2YzNmY2NTk0MmEyOTM4NGNmODk0MjZhNzQxZCIsImlhdCI6MTcyNTk3NTI2MSwiZXhwIjoyMDQxMzM1MjYxfQ.ylQDAWp0lEs1OGgjxGxAO7LYavuon-FuorspQhM8kDI";
            hassClient.token = _settings.hassToken;
            RuleEngine.GetInstance().AddClient(hassClient);
            
            _apiServer = new APIServer();
            //_apiServer = new APIServer("http://+", 5000);
            _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
        }
        catch (Exception e)
        {
            Debug.Log($"ERRORE NELLO START DI HOMEASSISTANTDEMORULES {e.Message}");
        }
    }

}
