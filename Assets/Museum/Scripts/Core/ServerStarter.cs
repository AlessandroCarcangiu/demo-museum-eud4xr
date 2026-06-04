using System.IO;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using UnityEngine;


public class ServerStarter : MonoBehaviour
{
    [System.Serializable]
    
    public class Settings
    {
        public string hassUrl;
        public string hassToken;
        public bool doLog;
        public string taskModellingUIVersion;
    }
    
    private APIServer _apiServer;

    public Settings settings;
    
    private string path;

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
        Debug.unityLogger.logEnabled = settings.doLog;
    }

    // Start is called before the first frame update
    void Start()
    {
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app"
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = settings.hassUrl;
        hassClient.token = settings.hassToken;
        RuleEngine.GetInstance().AddClient(hassClient);
        _apiServer = new APIServer(8123);
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
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