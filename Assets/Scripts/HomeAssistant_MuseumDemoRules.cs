using System;
using System.IO;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;


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
    //public float moveDistance = 2.0f;
    //private GameObject objectToMove;

    private void Awake()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "settings.json");
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
    }


    // Start is called before the first frame update
    void Start()
    { 
        // Home Assistant configuration
        // configure home assistant client
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = _settings.hassUrl;
        hassClient.token = _settings.hassToken;
        RuleEngine.GetInstance().AddClient(hassClient);
        
        // configure api server
        _apiServer = new APIServer();
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app"
            
        //objectToMove = GameObject.Find("Test_01");
        //objectToMove.transform.position.Set(0, 0, 0);

        //if (objectToMove == null) {
            //Debug.LogError("Nessun oggetto assegnato e 'objectToMove' non trovato nella scena.");
            //return;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDisable()
    {
        CloseServer();
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
