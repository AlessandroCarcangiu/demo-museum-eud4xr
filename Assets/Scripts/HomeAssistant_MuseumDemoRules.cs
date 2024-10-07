using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;

public class HomeAssistant_MuseumDemoRules : MonoBehaviour
{
    private APIServer _apiServer;
    public float moveDistance = 2.0f;
    private GameObject objectToMove;
    
    // Start is called before the first frame update
    void Start()
    {
        // Home Assistant configuration
        // configure home assistant client
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        hassClient.url = "http://127.0.0.1:8123";
        hassClient.token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiIyZDhjMDAwMTBlZmU0ZGE1YTEzYWI5YTdmMzlkYzVkMiIsImlhdCI6MTcyNzc5NTcwMSwiZXhwIjoyMDQzMTU1NzAxfQ.VQXdrlIKZBFn8RxsDVKAAFwSprt4VWPxh_QzXhA18ho";
        RuleEngine.GetInstance().AddClient(hassClient);
        
        // configure api server
        _apiServer = new APIServer();
        _apiServer.Update += ((HomeAssistantClient)hassClient).receivedUpdateHandler;
        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        
        objectToMove = GameObject.Find("Test_01");
        objectToMove.transform.position.Set(0, 0, 0);

        if (objectToMove == null)
        {
            Debug.LogError("Nessun oggetto assegnato e 'objectToMove' non trovato nella scena.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && objectToMove != null)
        {
            int randomAxis = Random.Range(0, 3);
            float randomDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
            
            Vector3 moveVector = Vector3.zero;
            Vector3 startingPosition = objectToMove.transform.position;

            switch (randomAxis)
            {
                case 0: // Asse X
                    moveVector.x = moveDistance * randomDirection;
                    break;
                case 1: // Asse Y
                    moveVector.y = moveDistance * randomDirection;
                    break;
                case 2: // Asse Z
                    moveVector.z = moveDistance * randomDirection;
                    break;
            }

            objectToMove.transform.position += moveVector;
            Vector3 finalPosition = objectToMove.transform.position;
            RuleEngine.GetInstance().ExecuteAction(new Action(
                objectToMove, 
                "moves to", 
                new Position(finalPosition.x, finalPosition.y, finalPosition.z)
                )
            );
            
            Debug.Log($"Moved {objectToMove.name} by {moveVector} units on axis {randomAxis} from {startingPosition} to {finalPosition}");
        }
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
