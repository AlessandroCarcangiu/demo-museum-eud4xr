using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;

public class HomeAssistant_MuseumDemoRules : MonoBehaviour
{
    private APIServer _apiServer;
    
    // Start is called before the first frame update
    void Start()
    {
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = "http://127.0.0.1:8123";
        hassClient.token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiIyZDhjMDAwMTBlZmU0ZGE1YTEzYWI5YTdmMzlkYzVkMiIsImlhdCI6MTcyNzc5NTcwMSwiZXhwIjoyMDQzMTU1NzAxfQ.VQXdrlIKZBFn8RxsDVKAAFwSprt4VWPxh_QzXhA18ho";
        RuleEngine.GetInstance().AddClient(hassClient);
        // configure api server
        _apiServer = new APIServer();
        _apiServer.Update += ((HomeAssistantClient)hassClient).receivedUpdateHandler;
        // from ngrok terminal digit and execute: ngrok http your_port --host-header="your_url:your_port" - example: ngrok http 8080 --host-header="localhost:8080"
    }

    // Update is called once per frame
    void Update()
    {
        // var pippo = ComponentTracker.Instance.GetAllComponents();
        // Debug.Log(pippo);
    }
}
