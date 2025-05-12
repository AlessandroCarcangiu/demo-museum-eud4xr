using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.UI;
using ECARules4All_DLL.Utils;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Action = ECARules4All_DLL.Action;

public class ECAUI_Action : MonoBehaviour
{
    //////////// UI Components ////////////
    public TextMeshProUGUI WhenThen_InputText;

    // Mandatory
    public TMP_Dropdown Subject_1_Dropdown;
    public TMP_Dropdown Verb_2_Dropdown;

    // Optionals
    public TMP_Dropdown Object_3_Dropdown;

    // public TMP_Dropdown Object_3A_Dropdown;
    // public TMP_Dropdown ObjectValue_3B_Dropdown;
    public TMP_Dropdown Preposition_4_Dropdown;
    public TMP_Dropdown Value_5A_Dropdown;
    public TMP_InputField Value_5B_InputText;

    // public Button deleteActionButton;
    public Button deleteActionButton;
    ///////////////////////////////////////


    ///////////// OTHER ? //////////////////
    ECAUI_Utils.ActionPlaceholder actionPlaceholder = new ECAUI_Utils.ActionPlaceholder();
    public ECAUI_Utils.ActionPlaceholder GetActionPlaceholder() => actionPlaceholder;
    ///////////////////////////////////////
    public enum ActionPreLabel
    {
        When,
        Then,
        None
    };

    private void SetActionLabelAsWhen(ActionPreLabel preLabel)
    {
        var s = Enum.GetName(typeof(ActionPreLabel), preLabel);
        if (s == null || s.ToLower() == "none")
        {
            s =  string.Empty;
        }
        WhenThen_InputText.text = s;
    }


    #region On Change Dropdowns

    public void HandleSubjectChange()
    {
        Debug.Log("Subject Dropdown Changed");
        var newSubject = Subject_1_Dropdown.options[Subject_1_Dropdown.value].text;

        // Clean the other fields or it may cause some errors. I do this by simply creating a new ActionPlaceholder
        actionPlaceholder = new ECAUI_Utils.ActionPlaceholder().SetChainSubject(newSubject);

        this.DrawView();
    }

    public void HandleVerbChange()
    {
        Debug.Log("Verb Dropdown Changed");

        var subject = Subject_1_Dropdown.options[Subject_1_Dropdown.value].text;
        var newVerb = Verb_2_Dropdown.options[Verb_2_Dropdown.value].text;

        // Clean the other fields or it may cause some errors. I do this by simply creating a new ActionPlaceholder
        actionPlaceholder = new ECAUI_Utils.ActionPlaceholder().SetChainSubject(subject).SetChainVerb(newVerb);

        this.DrawView();
    }

    public void HandleObjectChange()
    {
        Debug.Log("Object Dropdown Changed");

        var subject = Subject_1_Dropdown.options[Subject_1_Dropdown.value].text;
        var verb = Verb_2_Dropdown.options[Verb_2_Dropdown.value].text;
        var newObject = Object_3_Dropdown.options[Object_3_Dropdown.value].text;

        var actionAttributesList = infoCapabilities.allActionAttributes[subject][verb];
        var actionAttribute = actionAttributesList.Count > 1
            ? actionAttributesList.Find(obj => obj["variableName"] == newObject)
            : actionAttributesList[0];
        actionPlaceholder = new ECAUI_Utils.ActionPlaceholder()
            .SetChainSubject(subject)
            .SetChainVerb(verb)
            .SetChainObject(newObject)
            .SetChainActionAttributeInfo(actionAttribute);
        this.DrawView();
    }

    public void HandlePrepChange()
    {
        Debug.Log("Preposition Value Dropdown Changed");
        var newPrep = Preposition_4_Dropdown.options[Preposition_4_Dropdown.value].text;
        actionPlaceholder.SetChainPrep(newPrep);
        this.DrawView();
    }

    public void HandleValueDropdownChange()
    {
        var isDropdownEnabled = Value_5A_Dropdown.gameObject.activeSelf;
        Debug.Log(isDropdownEnabled ? "Value Dropdown Changed" : "Value Input Field Changed");
        var newValue = isDropdownEnabled
            ? Value_5A_Dropdown.options[Value_5A_Dropdown.value].text
            : Value_5B_InputText.text;
        actionPlaceholder.SetChainValue(newValue);
        this.DrawView();
    }

    public void HandleValueInputFieldChange()
    {
        Debug.Log("Preposition Dropdown Changed");
    }

    #endregion

    #region Enable/Disable

    public void OnEnable()
    {
        Debug.Log("AAA OnEnable" + gameObject.name);
        // SetListeners();
    }

    public void OnDisable()
    {
        Debug.Log("ZZZ OnDisable: " + gameObject.name);
        ClearFieldsAndTheirListeners();
    }

    private void SetListeners()
    {
        Subject_1_Dropdown.onValueChanged.AddListener(delegate { HandleSubjectChange(); });
        Verb_2_Dropdown.onValueChanged.AddListener(delegate { HandleVerbChange(); });
        Object_3_Dropdown.onValueChanged.AddListener(delegate { HandleObjectChange(); });
        Preposition_4_Dropdown.onValueChanged.AddListener(delegate { HandlePrepChange(); });
        Value_5A_Dropdown.onValueChanged.AddListener(delegate { HandleValueDropdownChange(); });
        Value_5B_InputText.onDeselect.AddListener(delegate { HandleValueInputFieldChange(); });
    }

    private void ClearFieldsAndTheirListeners()
    {
        ClearAllComponents();
        ClearListeners();
    }

    void ClearAllComponents()
    {
        // Clear all dropdowns, options in dropdowns, and input text
        // WhenThen_InputText.text = "";

        Subject_1_Dropdown.ClearOptions();
        Subject_1_Dropdown.gameObject.SetActive(true);

        Verb_2_Dropdown.ClearOptions();
        Verb_2_Dropdown.gameObject.SetActive(true);

        Object_3_Dropdown.ClearOptions();
        Object_3_Dropdown.gameObject.SetActive(false);

        Preposition_4_Dropdown.ClearOptions();
        Preposition_4_Dropdown.gameObject.SetActive(false);

        Value_5A_Dropdown.ClearOptions();
        Value_5A_Dropdown.gameObject.SetActive(false);

        Value_5B_InputText.text = "";
        Value_5B_InputText.gameObject.SetActive(false);
    }

    void ClearListeners()
    {
        Subject_1_Dropdown.onValueChanged.RemoveAllListeners();
        Verb_2_Dropdown.onValueChanged.RemoveAllListeners();
        Object_3_Dropdown.onValueChanged.RemoveAllListeners();
        // Object_3A_Dropdown.onValueChanged.RemoveAllListeners();
        // ObjectValue_3B_Dropdown.onValueChanged.RemoveAllListeners();
        Preposition_4_Dropdown.onValueChanged.RemoveAllListeners();
        Value_5A_Dropdown.onValueChanged.RemoveAllListeners();
        Value_5B_InputText.onValueChanged.RemoveAllListeners();
        // if (deleteActionButton.gameObject.activeSelf)
        // {
        //     deleteActionButton.onClick.RemoveAllListeners();
        // }
    }

    #endregion

    private ECAObjectInfo.ECAObjectsCapabilties infoCapabilities;

    public void SetUIParameters(ActionPreLabel preLabel, Action action, ECAObjectInfo.ECAObjectsCapabilties infoCapabilities, ECAUI_Rule containerRuleRef, bool enableDeleteButton = true)
{
    this.ClearFieldsAndTheirListeners();
    this.infoCapabilities = infoCapabilities;
    // this.containerRuleRef = containerRuleRef;

    SetActionLabelAsWhen(preLabel);
    if (enableDeleteButton)
    {
        deleteActionButton.gameObject.SetActive(true);
        deleteActionButton.onClick.AddListener(() =>
        {
            Debug.Log("CLICK DELETE ACTION");
            containerRuleRef?.RemoveThenAction(this);
        });
    }
    else
    {
        deleteActionButton.gameObject.SetActive(false);
    }

    if (action == null) 
    {
        throw new NotImplementedException(
            "Action is null. Why? Do we want to create Rules from the UI from scratch?");
    }

    // Load the current action's values into the dropdowns
    actionPlaceholder.SetFromAction(action);

    // Draw the UI
    this.DrawView();
}


    private void DrawView()
    {
        ClearFieldsAndTheirListeners();
        // this.ClearListeners();

        List<string> EvaluateVerbs(string subject)
        {
            if (!infoCapabilities.allActionAttributes.ContainsKey(subject))
                return new List<string> { "Verbs not found" };
            var x = infoCapabilities.allActionAttributes[subject].Keys.OrderBy(x => x).ToList();
            return x;
        }


        var subject = actionPlaceholder.Subject;
        var verb = actionPlaceholder.Verb;
        var obj = actionPlaceholder.Object;
        var prep = actionPlaceholder.Prep;
        var value = actionPlaceholder.Value;


        var isSubjectInserted = !string.IsNullOrEmpty(subject);
        var isVerbInserted = !string.IsNullOrEmpty(verb);
        var isObjectInserted = !string.IsNullOrEmpty(obj);
        var isPrepInserted = !string.IsNullOrEmpty(prep);
        var isValueInserted = !string.IsNullOrEmpty(value);

        /**
        *          <Dropdown className={classes.dropdown}
                    value={subject}
                    options={subjects}
                    onChange={handleSubjectChange}
                    placeholder="Select the subject"/>
                <Dropdown disabled={!subjectInserted}
                    className={classes.dropdown}
                    value={verb}
                    options={evalVerbs(subject)}
                    onChange={handleVerbChange}
                    placeholder="Select the verb"/>
         */
        Subject_1_Dropdown.gameObject.SetActive(true);
        Subject_1_Dropdown.ClearOptions();
        Subject_1_Dropdown.AddOptions(infoCapabilities.allActionAttributes.Keys.ToList());
        // Subject_1_Dropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select the subject";
        if (isSubjectInserted)
        {
            Subject_1_Dropdown.value = Subject_1_Dropdown.options.FindIndex(option => option.text == subject);
        }

        Verb_2_Dropdown.gameObject.SetActive(true);
        Verb_2_Dropdown.ClearOptions();
        Verb_2_Dropdown.AddOptions(EvaluateVerbs(subject));
        // Verb_2_Dropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select the verb";
        if (isVerbInserted)
        {
            Verb_2_Dropdown.value = Verb_2_Dropdown.options.FindIndex(option => option.text == verb);
        }


        void SetComponentsAfterVerb()
        {
            /**
             *         const objectInserted = object !== undefined && object !== null && object !== "";
        const prepInserted = prep !== undefined && prep !== null && prep !== "";
        const valueInserted = value !== undefined && value !== null && !isNaN(value);
             */
            var objectInserted = !string.IsNullOrEmpty(obj);
            var prepInserted = !string.IsNullOrEmpty(prep);
            var valueInserted = !string.IsNullOrEmpty(value);

            /**
             *         const getObjectOptions = (s, v) => {
            let options = []
            if (!s || !v) return options

            const dropdowns_after_verb = ctx.info.allDropdownOptionsAfterVerbs[s][v]

            if (dropdowns_after_verb[DROPDOWN_OBJECT])
                options = dropdowns_after_verb[DROPDOWN_OBJECT]
            else if (dropdowns_after_verb[DROPDOWN_OBJECT_VALUE])
                options = dropdowns_after_verb[DROPDOWN_OBJECT_VALUE]

            return options
        }
             */
            List<string> GetObjectOptions(string s, string v)
            {
                var options = new List<string>();
                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(v)) return options;

                var dropdownsAfterVerb =
                    infoCapabilities.allDropdownOptionsAfterVerbs[s][v]; //todo error changes intensity

                if (dropdownsAfterVerb.ContainsKey(ECAUI_Utils.DROPDOWN_3A_OBJECT))
                    options = dropdownsAfterVerb[ECAUI_Utils.DROPDOWN_3A_OBJECT];
                else if (dropdownsAfterVerb.ContainsKey(ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE))
                    options = dropdownsAfterVerb[ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE];
                else
                {
                    // if dropdownsAfterVerb keys are more than 0, throw an exception
                    if (dropdownsAfterVerb.Keys.Count > 0)
                    {
                        throw new Exception(
                            $"{dropdownsAfterVerb} does not contain {ECAUI_Utils.DROPDOWN_3A_OBJECT} or {ECAUI_Utils.DROPDOWN_3B_OBJECT_VALUE}");
                    }
                }
                // append empty string as first element
                // options.Insert(0, ""); //TODO Better to use the Dropdown placeholder?

                return options;
            }

            /**
             *         const TryToGetPrepositionOptions = (s, v, o) => {
            output = {'isTherePrep': false, 'optionsWithSelectedObject': []}

            if (!s || !v) {
                return output
            }
            output.isTherePrep = Object.keys(ctx.info.allDropdownOptionsAfterObjects[s][v]).length > 0
            if (!o) return output
            if (!output.isTherePrep) return output

            const tmp = ctx.info.allDropdownOptionsAfterObjects[s][v][o]

            if (tmp[DROPDOWN_PREPOSITION])
                output.optionsWithSelectedObject = tmp[DROPDOWN_PREPOSITION]

            return output
        }
             */
            (bool isTherePrep, List<string> optionsWithSelectedObject) TryToGetPrepositionOptions(string s, string v,
                string o)
            {
                var output = (isTherePrep: false, optionsWithSelectedObject: new List<string>());
                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(v)) return output;

                output.isTherePrep = (infoCapabilities.allDropdownOptionsAfterObjects[s][v]).Keys.Count > 0;
                if (string.IsNullOrEmpty(o)) return output;
                if (!output.isTherePrep) return output;

                var tmp = infoCapabilities.allDropdownOptionsAfterObjects[s][v][o];
                if (tmp.ContainsKey(ECAUI_Utils.DROPDOWN_4_PREPOSITION))
                    output.optionsWithSelectedObject = tmp[ECAUI_Utils.DROPDOWN_4_PREPOSITION];

                return output;
            }

            /**
             *         const TryToGetValueOptions = (s, v, o, p) => {
            output = {'isThereValue': false, 'type': 'dropdown'}

            if (!s || !v) {
                return output
            }
            output.isThereValue = Object.keys(ctx.info.allDropdownOptionsAfterObjects[s][v]).length > 0
            if (!o) return output
            if (!output.isThereValue) return output

            const tmp = ctx.info.allDropdownOptionsAfterObjects[s][v][o]

            if (tmp[DROPDOWN_VALUE]) {
                output.optionsWithSelectedObject = tmp[DROPDOWN_VALUE]
            } else if (tmp[DROPDOWN_INPUT_FIELD]) {
                output.optionsWithSelectedObject = tmp[DROPDOWN_INPUT_FIELD]
                output.type = 'input'
                output.InputTypeOriginal = tmp[DROPDOWN_INPUT_FIELD]
                output.InputTypeMappedForPrimeReact = "text"
            } else if (tmp[FETCH_MEDIA]) {
                switch (tmp[FETCH_MEDIA].toString()) {
                    case "images":
                        output.optionsWithSelectedObject = images
                        break;
                    case "audios":
                        output.optionsWithSelectedObject = audios
                        break;
                    case "videos":
                        output.optionsWithSelectedObject = videos
                        break;
                    default:
                        output.optionsWithSelectedObject = []
                        console.warn("Media type not supported: " + tmp[FETCH_MEDIA])
                        break;
                }
            } else {
                return {'isThereValue': false, 'type': 'dropdown', 'optionsWithSelectedObject': []}
            }


            return output
        }
             */
            (bool isThereValue, string type, List<string> options) TryToGetValueOptions(string s, string v, string o,
                string p)
            {
                var output = (isThereValue: false, type: "dropdown", options: new List<string>());

                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(v) || string.IsNullOrEmpty(o)) return output;

                output.isThereValue = (infoCapabilities.allDropdownOptionsAfterObjects[s][v]).Keys.Count > 0;
                if (!output.isThereValue) return output;

                var tmp = infoCapabilities.allDropdownOptionsAfterObjects[s][v][o];
                if (tmp.ContainsKey(ECAUI_Utils.DROPDOWN_5A_VALUE))
                {
                    output.options = tmp[ECAUI_Utils.DROPDOWN_5A_VALUE];
                }
                else if (tmp.ContainsKey(ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD))
                {
                    output.options = tmp[ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD];
                    output.type = "input";
                    // output.InputTypeOriginal = tmp[ECAUI_Utils.DROPDOWN_5B_INPUT_FIELD]; //todo Do I care about this?
                }
                else if (tmp.ContainsKey(ECAUI_Utils.FETCH_MEDIA))
                {
                    switch (tmp[ECAUI_Utils.FETCH_MEDIA].ToString())
                    {
                        case "images":
                            var images = new List<string>()
                                { "MEMO IMPLEMENT THE REAL RETRIEVAL OF THE IMAGE FILE LIST" }; //todo temp?
                            output.options = images;
                            break;
                        case "audios":
                            var audios = new List<string>()
                                { "MEMO IMPLEMENT THE REAL RETRIEVAL OF THE AUDIO FILE LIST" }; //TODO temp? 
                            output.options = audios;
                            break;
                        case "videos":
                            var videos = new List<string>()
                                { "MEMO IMPLEMENT THE REAL RETRIEVAL OF THE VIDEO FILE LIST" }; //TODO temp?
                            output.options = videos;
                            break;
                        default:
                            output.options = new List<string>();
                            Debug.LogWarning("Media type not supported: " + tmp[ECAUI_Utils.FETCH_MEDIA]);
                            break;
                    }
                }
                else
                {
                    return (isThereValue: false, type: "dropdown", options: new List<string>());
                }

                return output;
            }

            /**
             *         let output = <></>
        if (!(subjectInserted && verbInserted)) return output;
             */
            if (!(isSubjectInserted && isVerbInserted))
            {
                Object_3_Dropdown.gameObject.SetActive(false);
                Preposition_4_Dropdown.gameObject.SetActive(false);
                Value_5A_Dropdown.gameObject.SetActive(false);
                Value_5B_InputText.gameObject.SetActive(false);
            }

            /**
             *         const objDropdownOptions = getObjectOptions(subject, verb)
        const objectDropdown = objDropdownOptions.length === 0 ? <></>
            :
            <Dropdown
                className={classes.dropdown}
                value={object}
                options={objDropdownOptions}
                onChange={handleObjectChange}
                placeholder="Select the object"/>;
             */
            var objDropdownOptions = GetObjectOptions(subject, verb);
            if (objDropdownOptions.Count == 0)
            {
                Object_3_Dropdown.gameObject.SetActive(false);
                Preposition_4_Dropdown.gameObject.SetActive(false);
                Value_5A_Dropdown.gameObject.SetActive(false);
                Value_5B_InputText.gameObject.SetActive(false);
            }
            else
            {
                Object_3_Dropdown.gameObject.SetActive(true);
                Object_3_Dropdown.ClearOptions();
                Object_3_Dropdown.AddOptions(objDropdownOptions);
                // Object_3_Dropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select the object";
                if (isObjectInserted)
                {
                    // split string "AcquaticAnimal Fish" into "AcquaticAnimal" and "Fish"
                    Object_3_Dropdown.value = Object_3_Dropdown.options.FindIndex(option =>
                        option.text == obj || option.text.Contains(" " + obj));
                }
            }

            /**
             *         const {isTherePrep, optionsWithSelectedObject} = TryToGetPrepositionOptions(subject, verb, object);
        if (!isTherePrep) return objectDropdown
        const prepositionDropdown =
            <Dropdown disabled={!(objectInserted || prepInserted)}
                      className={classes.dropdown}
                      value={prep}
                      options={optionsWithSelectedObject}
                      onChange={handlePrepChange}
                      placeholder="Select the object"/>
             */
            var (isTherePrep, prepDropdownOptions) = TryToGetPrepositionOptions(subject, verb, obj);
            if (!isTherePrep)
            {
                Preposition_4_Dropdown.gameObject.SetActive(false);
                Value_5A_Dropdown.gameObject.SetActive(false);
                Value_5B_InputText.gameObject.SetActive(false);
            }
            else
            {
                Preposition_4_Dropdown.gameObject.SetActive(true);
                Preposition_4_Dropdown.ClearOptions();
                Preposition_4_Dropdown.AddOptions(prepDropdownOptions);
                // Preposition_4_Dropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select the preposition";
                if (isPrepInserted)
                {
                    Preposition_4_Dropdown.value =
                        Preposition_4_Dropdown.options.FindIndex(option => option.text == prep);
                }

                Preposition_4_Dropdown.interactable = (objectInserted || prepInserted);
            }

            /**
        const objValue = TryToGetValueOptions(subject, verb, object, prep);
        if (!objValue.isThereValue) return <>{objectDropdown}{prepositionDropdown}</>
        let valueDropdown = NaN
        if (objValue.type === 'dropdown')
            valueDropdown = <Dropdown disabled={!(objectInserted && prepInserted)}
                                      className={classes.dropdown}
                                      value={value}
                                      options={objValue.optionsWithSelectedObject}
                                      onChange={handleValueChange}
                                      placeholder="Select the object"/>
        else
            valueDropdown = <InputText disabled={!(objectInserted && prepInserted)}
                // className={classes.dropdown}
                                       value={value}
                                       type={output.InputTypeMappedForPrimeReact}
                                       onChange={handleValueChange}
                                       placeholder="Select the object"/>
        return <>{objectDropdown}{prepositionDropdown}{valueDropdown}</>
         */
            var objValue = TryToGetValueOptions(subject, verb, obj, prep);
            if (!objValue.isThereValue)
            {
                Value_5A_Dropdown.gameObject.SetActive(false);
                Value_5B_InputText.gameObject.SetActive(false);
            }
            else
            {
                if (objValue.type == "dropdown")
                {
                    Value_5A_Dropdown.gameObject.SetActive(true);
                    Value_5B_InputText.gameObject.SetActive(false);
                    Value_5A_Dropdown.ClearOptions();
                    Value_5A_Dropdown.AddOptions(objValue.options);
                    if (isValueInserted)
                    {
                        Value_5A_Dropdown.value = Value_5A_Dropdown.options.FindIndex(option => option.text == value);
                    }
                    // Value_5A_Dropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select the value";
                    // Value_5A_Dropdown.interactable = (objectInserted && prepInserted); //todo do I want this?
                }
                else
                {
                    Value_5A_Dropdown.gameObject.SetActive(false);
                    Value_5B_InputText.gameObject.SetActive(true);
                    Value_5B_InputText.text = value;
                    // Value_5B_InputText.interactable = (objectInserted && prepInserted); //todo do I want this?
                }
            }
        }

        if (isSubjectInserted && isVerbInserted)
        {
            SetComponentsAfterVerb();
        }

        this.SetListeners();
    }
}