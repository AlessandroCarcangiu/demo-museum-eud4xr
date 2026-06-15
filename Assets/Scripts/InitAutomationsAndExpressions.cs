using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL;
using ECARules4All_DLL.Logger;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Action = ECARules4All_DLL.Action;
using APIServer = ECARules4All_DLL.SmartHomeHubClients.APIServer;


public class InitAutomationsAndExpressions : MonoBehaviour
{
    private APIServer _apiServer;
    //private APIServerV2 _apiServer;

    private string path;

    private bool isStarted = false;
    public bool IsStarted => isStarted;

    public Dictionary<string, string> automations = new Dictionary<string, string>();

    async void Awake()
    {
        // open settings file
        string jsonString = File.ReadAllText("Assets/StreamingAssets/hass-settings.json");
        var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        
        // seq settings
        /*
        LoggingOptions opt = new LoggingOptions
        {
            LogFilePath = "logs/ecarules4all-logs.txt",
            SeqUrlToken = new SeqUrlToken(settings["seqUrl"], settings["seqApiKey"])
        };
        RuleEngine.ApplyLoggingOptions(opt);
        */
        // hass settings
        
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app" - unity
        // ngrok http 8123 --host-header="localhost:8123" --domain="fly-powerful-slug.ngrok-free.app" - hass 

        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        //AbstractClient<HomeAssistantClientV2> hassClient = AbstractClient<HomeAssistantClientV2>.GetInstance();
        
        hassClient.url = settings["hassUrl"];
        hassClient.token = settings["hassToken"];
        RuleEngine.GetInstance().AddClient(hassClient);
        
        _apiServer = new APIServer(8080);
        //_apiServer = new APIServerV2(8080);
        
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
        //_apiServer.ActionUpdate += ((HomeAssistantClientV2)hassClient).ReceivedUpdateHandler;
        
        // get automations and expressions 
        var expressions = await hassClient.GetListExpressions();
        var automations = await hassClient.GetListAutomationsWithDescription();
        Debug.Log($"Received {expressions.Count} expressions");
        Debug.Log($"Received {automations.Count} automations");

        FirstPrototype.UIExpressions.Instance.HandleExpressions(expressions, automations);

        SecondPrototype.UIExpressions.Instance.HandleExpressions(expressions, automations);
        
        this.isStarted = true;
        
        Debug.Log("Server started");
    }
}