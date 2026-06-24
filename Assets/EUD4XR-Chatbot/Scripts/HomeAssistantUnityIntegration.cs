using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL;
using ECARules4All_DLL.Logger;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Taverna;
using ECARules4All_DLL.Utils;
using EUD4XR_Chatbot.TaskModelling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Action = ECARules4All_DLL.Action;
using APIServer = ECARules4All_DLL.SmartHomeHubClients.APIServer;


[RequireComponent(typeof(IotDeviceManager))]
public class HomeAssistantUnityIntegration : Singleton<HomeAssistantUnityIntegration>
{
    // public string filename = "HASS-Settings.json";

    private APIServer _apiServer;
    //private APIServerV2 _apiServer;

    private HASS_Settings settings;
    // public HASS_Settings Settings => settings;

    private string path;

    private bool isStarted = false;
    public bool IsStarted => isStarted;

    public Dictionary<string, string> automations = new Dictionary<string, string>();

    protected override void OnAwake()
    {
        Debug.Log("[HomeAssistantUnityIntegration OnAwake] start");
        base.OnAwake();

        StartCoroutine(OnSettingsAvailable());
        Debug.Log("[HomeAssistantUnityIntegration OnAwake] end");
    }

    void Start()
    {
        StartCoroutine(GetExpressionsAndAutomations());
    }

    IEnumerator OnSettingsAvailable()
    {
        yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);

        // seq settings
        var seqSettings = DualChatbotServerSettings.Instance.GetSettings_SEQ();
        LoggingOptions opt = new LoggingOptions
        {
            LogFilePath = "logs/ecarules4all-logs.txt",
            SeqUrlToken = new SeqUrlToken(seqSettings.url, seqSettings.apiKey)
        };
        RuleEngine.ApplyLoggingOptions(opt);

        // hass settings
        this.settings = DualChatbotServerSettings.Instance.GetSettings_HASS();

        if (this.settings == null)
            throw new Exception("Home Assistant settings are null.");

        if (string.IsNullOrEmpty(this.settings.token))
            throw new Exception("Home Assistant URL or token are not set in the settings.");

        if (string.IsNullOrEmpty(this.settings.url))
            throw new Exception("Home Assistant URL or token are not set in the settings.");


        if (!this.settings.url.EndsWith("/"))
        {
            this.settings.url += "/";
        }

        Debug.unityLogger.logEnabled = settings.doLog;
        Debug.Log($"[HomeAssistantUnityIntegration - OnSettingsAvailable] HASS URL from settings: {settings.url}");

        // from ngrok terminal digit and execute:
        // ngrok http your_port --host-header="your_url:your_port" -
        // example: ngrok http 8080 --host-header="localhost:8080"
        // from ngrok static url:
        // ngrok http 8080 --host-header="localhost:8080" --domain="fly-powerful-slug.ngrok-free.app" - unity
        // ngrok http 8123 --host-header="localhost:8123" --domain="fly-powerful-slug.ngrok-free.app" - hass 

        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        //AbstractClient<HomeAssistantClientV2> hassClient = AbstractClient<HomeAssistantClientV2>.GetInstance();
        
        hassClient.url = settings.url;
        hassClient.token = settings.token;
        RuleEngine.GetInstance().AddClient(hassClient);
        
        _apiServer = new APIServer(8080);
        //_apiServer = new APIServerV2(8080);
        
        _apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;
        //_apiServer.ActionUpdate += ((HomeAssistantClientV2)hassClient).ReceivedUpdateHandler;
        
        // register to ReceivedAutomations and ReceivedExpressions
        _apiServer.RegisteredAutomations += this.UpdateTaskModellingCards;
        //_apiServer.RegisteredExpressions += this.UpdateTaskModellingCards;
        this.isStarted = true;
    }

    private void UpdateTaskModellingCards(object sender, List<Expression> e)
    {
        StartCoroutine(EndpointUtils_Get_Expressions(expressions =>
            {
                StartCoroutine(Async_Get_Automation_Info_As_Dict(automations =>
                    {
                        this.automations = automations;

                        //ExpressionController.Instance.RenderExpression(expressions[0]);
                    })
                );
            })
        );
    }

    private void UpdateTaskModellingCards(object sender, List<AutomationDTO> e)
    {
        StartCoroutine(Async_Get_Automation_Info_As_Dict(atms =>
            {
                this.automations = atms;
                
            })
        );
        /*StartCoroutine(EndpointUtils_Get_Expressions(expressions =>
            {
                StartCoroutine(Async_Get_Automation_Info_As_Dict(automations =>
                    {
                        this.automations = automations;

                        ExpressionController.Instance.RenderExpression(expressions[0]);
                    })
                );
            })
        );*/
    }

    private string GetTokenSafe()
    {
        var serverToken = HomeAssistantUnityIntegration.Instance.settings.token;
        if (string.IsNullOrEmpty(serverToken))
        {
            throw new Exception("Home Assistant URL is not set in the settings.");
        }

        return serverToken;
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
        if (_apiServer != null)
        {
            _apiServer.Stop();
        }
    }

    private IEnumerator GetExpressionsAndAutomations()
    {
        JArray expressions = null;

        yield return EndpointUtils_Get_Expressions(expr => { expressions = expr; });

        yield return Async_Get_Automation_Info_As_Dict(auts => { this.automations = auts; });

        if (expressions == null || expressions.Count == 0)
        {
            Debug.LogWarning("No expressions found from Home Assistant or server is unreachable.");
            yield break;
        }

        if (ExpressionController.Instance != null)
        {
            ExpressionController.Instance.RenderExpression(expressions[0]);
        }
    }

    //region Endpoints
    public IEnumerator EndpointUtils_Update_IotDevice_Visibility(
        (string sensor_name, (float x, float y, float z) position, bool isInsideCamera) payload,
        System.Action onErrorCallback, System.Action onSuccessCallback)
    {
        const string endpoint = "api/eud4xr/update_iotdevice_visibility_from_unity";
        string url = this.settings.url + endpoint;

        // Create the JSON payload
        var jsonPayload = new
        {
            sensor_name = payload.sensor_name,
            position = new
            {
                x = payload.position.x,
                y = payload.position.y,
                z = payload.position.z
            },
            isInsideCamera = payload.isInsideCamera
        };
        string jsonString = JsonConvert.SerializeObject(jsonPayload, Formatting.Indented, new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        // Create the UnityWebRequest
        UnityWebRequest uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonString);

        // Use UploadHandlerRaw for sending the JSON payload
        uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + this.GetTokenSafe());

        // Send the request
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[IotDevice] Error in Updating {gameObject.name}: {uwr.error}");
            onErrorCallback?.Invoke();
        }
        else
        {
            onSuccessCallback?.Invoke();
        }
    }

    public IEnumerator EndpointUtils_Get_IotDevices_Capabilities(System.Action<string> onSuccessCallback)
    {
        yield return new WaitUntil(() => this.isStarted);
        const string endpoint = "api/eud4xr/list_real_objects_capabilities";
        string url = this.settings.url + endpoint;

        yield return UnityWebRequestUtils.GET_JSON(url, this.GetTokenSafe(), onSuccessCallback);
    }

    public IEnumerator EndpointUtils_Get_Expressions(System.Action<JArray> onSuccessCallback)
    {
        yield return new WaitUntil(() => this.isStarted);

        const string endpoint = "api/eud4xr/expressions";
        string url = this.settings.url + endpoint;

        void MyCustomCallback(string jsonResponse)
        {
            JArray expressions = null;
            var root = JObject.Parse(jsonResponse);
            var exprs = root["expressions"] as JObject;
            var allList = exprs.Properties()
                .Where(p => p.Value is JArray)
                .SelectMany(p => p.Value)
                .OfType<JObject>()
                .ToList();
            expressions = JArray.FromObject(allList);
            onSuccessCallback(expressions);
        }

        yield return UnityWebRequestUtils.GET_JSON(url, this.GetTokenSafe(), MyCustomCallback);
    }

    public IEnumerator EndpointUtils_Get_Automations(System.Action<List<object>> onSuccessCallback)
    {
        yield return new WaitUntil(() => this.isStarted);

        const string endpoint = "api/eud4xr/automations";
        string url = this.settings.url + endpoint;

        void MyCustomCallback(string jsonResponse)
        {
            List<object> ConvertAutomationsToRules(List<AutomationDTO> automations)
            {
                var rules = new List<object>();

                foreach (var a in automations)
                {
                    // rules.Add(a.ConvertToRule());
                    rules.Add(a.ConvertToRule());
                }

                return rules;
            }

            var rules = new List<object>();

            JObject jsonObject = JObject.Parse(jsonResponse);
            var a = jsonObject["automations"]?.ToString();
            Debug.Log($"Received Automations: {a}");
            List<AutomationDTO> automations =
                JsonConvert.DeserializeObject<List<AutomationDTO>>(jsonObject["automations"]?.ToString() ??
                                                                   throw new InvalidOperationException());
            rules = ConvertAutomationsToRules(automations);

            Debug.Log($"Receive automations list by contacting {url}");
            onSuccessCallback(rules);
        }

        yield return UnityWebRequestUtils.GET_JSON(url, this.GetTokenSafe(), MyCustomCallback);
    }
    //endregion

    /**
     *
     */
    public IEnumerator Async_Get_Automation_Info_As_Dict(System.Action<Dictionary<string, string>> onSuccessCallback,
        List<string> onlyRulesWithinThisList = null)
    {
        yield return new WaitUntil(() => this.isStarted);

        const string endpoint = "api/eud4xr/automations";
        string url = this.settings.url + endpoint;

        void MyCustomCallback(string jsonResponse)
        {
            Dictionary<string, string> ConvertAutomationsToRules(List<AutomationDTO2> automations)
            {
                var d = new Dictionary<string, string>();

                foreach (var a in automations)
                {
                    var k = a.alias;
                    string v = string.Empty;
                    if (onlyRulesWithinThisList != null && onlyRulesWithinThisList.Count > 0 &&
                        !onlyRulesWithinThisList.Contains(k))
                    {
                        continue;
                    }

                    var tmp = a.ConvertToRule();

                    if (tmp is ECARules4All_DLL.Rule rule)
                    {
                        v = RuleUtils.FormatRuleLabel(rule);
                    }
                    else if (tmp is System.String str)
                    {
                        v = str;
                    }
                    else
                    {
                        throw new Exception("Unexpected rule type");
                    }

                    d.Add(k, v);
                }

                return d;
            }

            var rules = new Dictionary<string, string>();

            JObject jsonObject = JObject.Parse(jsonResponse);
            var a = jsonObject["automations"]?.ToString();
            Debug.Log($"Received Automations: {a}");
            List<AutomationDTO2> automations =
                JsonConvert.DeserializeObject<List<AutomationDTO2>>(jsonObject["automations"]?.ToString() ??
                                                                    throw new InvalidOperationException());
            rules = ConvertAutomationsToRules(automations);

            Debug.Log($"Receive automations list by contacting {url}");
            onSuccessCallback(rules);
        }

        yield return UnityWebRequestUtils.GET_JSON(url, this.GetTokenSafe(), MyCustomCallback);
    }

    //region automation
    void Async_GetRulesInfoInvolvedInExpressions(bool fast)
    {
        if (fast)
        {
            StartCoroutine(
                HomeAssistantUnityIntegration.Instance.Async_Get_Automation_Info_As_Dict(
                    (d =>
                    {
                        // log the dictionary d
                        Debug.Log("DIZIONARIO: " + d);
                        this.automations = d;
                    })
                    // automations
                )
            );
        }
        else
        {
            StartCoroutine(
                HomeAssistantUnityIntegration.Instance.EndpointUtils_Get_Expressions(
                    (expressions =>
                    {
                        Debug.Log("Fatto. Expressions count: " + expressions.Count);


                        var i_want_only_this_automation = GetRulesInvolvedInExpressions(expressions);

                        StartCoroutine(
                            HomeAssistantUnityIntegration.Instance.Async_Get_Automation_Info_As_Dict(
                                (d =>
                                {
                                    // log the dictionary d
                                    Debug.Log("DIZIONARIO: " + d);
                                    this.automations = d;
                                }),
                                i_want_only_this_automation
                            )
                        );
                    })
                )
            );
        }
    }

    /**
     * Extracts all automation identifiers from a JSON string.
     *
     * The method searches the JSON structure recursively for string values
     * that begin with the prefix "automation." and returns only the part
     * after the prefix (e.g., "r1" instead of "automation.r1").
     *
     * @param json A JSON string containing an "expressions" object
     *             with arrays like "sequences", "orders", "choices", etc.
     *
     * @return A list of automation identifiers (e.g., ["r1", "r2", "r3"...]).
     *         Duplicates are preserved in the order they appear in the JSON.
     */
    List<string> GetRulesInvolvedInExpressions(JArray expressions)
    {
        var automations = new List<string>();

        // Traverse all arrays inside "expressions"
        foreach (var token in expressions.DescendantsAndSelf())
        {
            if (token.Type == JTokenType.String)
            {
                string value = token.ToString();
                if (value.StartsWith("automation."))
                {
                    // automations.Add(value);
                    automations.Add(
                        value.Substring("automation.".Length)); // Take only the part after "automation."
                }
            }
        }

        return automations;
    }
    //endregion
}





public class AutomationDTO2
{
    [JsonConverter(typeof(ObjectToDtoConverter<ActionDTO>))]
    public List<object> trigger { get; set; } = new List<object>();

    [JsonConverter(typeof(ObjectToDtoConverter<ConditionDTO>))]
    public object conditions { get; set; } = null;

    [JsonConverter(typeof(ObjectToDtoConverter<ActionDTO>))]
    public List<object> actions { get; set; } = new List<object>();

    public string id { get; set; } = null;
    public string alias { get; set; } = null;
    public string description { get; set; } = null;

    public object ConvertToRule()
    {
        object rule = null;
        // trigger
        object objTrigger;
        if (this.trigger[0] is ActionDTO)
        {
            objTrigger = ((ActionDTO)this.trigger[0]).ConvertToAction();
        }
        else
        {
            objTrigger = this.trigger[0];
        }

        // actions
        List<object> objActions = new List<object>();
        foreach (var action in this.actions)
        {
            if (action is ActionDTO)
            {
                objActions.Add(((ActionDTO)action).ConvertToAction());
            }
            else
            {
                objActions.Add(action);
            }
        }

        // conditions
        object objConditions = this.conditions;
        if (this.conditions != null && this.conditions is ConditionDTO)
        {
            objConditions = ((ConditionDTO)this.conditions).ConvertToCondition();
        }

        if (objTrigger is Action ecaTrigger &&
            (objConditions == null || (objConditions != null && objConditions is ECARules4All_DLL.Condition)) &&
            objActions.All(x => x is Action))
        {
            List<Action> ecaActions = objActions.Select(x => x as Action).ToList();
            if (objConditions != null)
            {
                rule = Rule.TryCreateRule(
                    ecaTrigger,
                    (ECARules4All_DLL.Condition)objConditions,
                    ecaActions
                );
            }
            else
            {
                rule = Rule.TryCreateRule((Action)objTrigger, objActions.Select(x => x as Action).ToList());
            }
        }
        else
        {
            if (objTrigger is Action)
            {
                rule += objTrigger.ToString();
            }
            else
            {
                rule += "when " + jsonToDictionary((JObject)objTrigger) + "\n";
            }

            if (objConditions != null)
            {
                if (objConditions is ECARules4All_DLL.Condition)
                {
                    rule += objConditions.ToString();
                }
                else
                {
                    rule += "if " + jsonToDictionary((JObject)objConditions) + "\n";
                }
            }

            foreach (var action in objActions)
            {
                if (action is Action)
                {
                    rule += action.ToString();
                }
                else
                {
                    rule += "then " + jsonToDictionary((JObject)action) + "\n";
                }
            }
        }

        return rule;
    }

    public static Type FindTypeByName(string className)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    public static MethodInfo FindMethodWithVerb(Type targetType, string verb, string variable = null)
    {
        MethodInfo[] methods =
            targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        List<MethodInfo> fMethods = new List<MethodInfo>();
        foreach (var method in methods)
        {
            // Check if the method has the ECAActionAttribute
            var attribute = method.GetCustomAttribute<ActionAttribute>();
            if (attribute != null)
            {
                bool verbComparison = attribute.Verb.Equals(verb, StringComparison.OrdinalIgnoreCase);
                // Compare the second argument (string) with the given input string
                if (verbComparison && String.IsNullOrEmpty(variable) ||
                    verbComparison && attribute.variableName.Equals(variable, StringComparison.OrdinalIgnoreCase))
                {
                    return method;
                }
            }
        }

        return null;
    }

    public static PropertyInfo FindPropertyByName(Type targetType, string propertyName)
    {
        PropertyInfo[] properties = targetType.GetProperties();
        foreach (var property in properties)
        {
            // Check if the method has the ECAActionAttribute
            var attribute = property.GetCustomAttribute<StateVariableAttribute>();
            if (attribute != null)
            {
                bool verbComparison = attribute.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
                // Compare the second argument (string) with the given input string
                if (verbComparison)
                {
                    return property;
                }
            }
        }

        return null;
    }

    public static string jsonToDictionary(JObject jObj)
    {
        string v = "";
        if (jObj != null)
            v = string.Join(" ", 
                jObj.Properties().Select(p => $"{p.Name}: {p.Value}; "));
        return v;
    }
}

public class ObjectToDtoConverter<T> : JsonConverter
{
    private static readonly HashSet<string> _dtoPropNames =
        new HashSet<string>(
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name),
            StringComparer.OrdinalIgnoreCase
        );

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(object) || objectType == typeof(List<object>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (objectType == typeof(List<object>))
        {
            var result = new List<object>();
            if (token.Type == JTokenType.Array)
            {
                foreach (var item in (JArray)token)
                    result.Add(ConvertItem(item, serializer));
            }
            else
            {
                result.Add(ConvertItem(token, serializer));
            }

            return result;
        }

        return ConvertItem(token, serializer);
    }

    private object ConvertItem(JToken item, JsonSerializer serializer)
    {
        switch (item.Type)
        {
            case JTokenType.Object:
            {
                var obj = (JObject)item;
                var jsonKeys = new HashSet<string>(obj.Properties().Select(p => p.Name),
                    StringComparer.OrdinalIgnoreCase);
                bool looksLikeDto = jsonKeys.Overlaps(_dtoPropNames);
                if (!looksLikeDto)
                    return obj;
                try
                {
                    return obj.ToObject<T>(serializer);
                }
                catch
                {
                    return obj;
                }
            }

            case JTokenType.Array:
            {
                var list = new List<object>();
                foreach (var child in (JArray)item)
                    list.Add(ConvertItem(child, serializer));
                return list;
            }

            case JTokenType.Null:
                return null;

            default:
                return ((JValue)item).Value;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JToken.FromObject(value, serializer).WriteTo(writer);
    }
}