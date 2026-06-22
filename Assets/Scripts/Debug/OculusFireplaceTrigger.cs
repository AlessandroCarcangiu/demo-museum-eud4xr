using UnityEngine;
using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Objects.Taverna;
using ECARules4All_DLL.Utils;

public class OculusFireplaceTrigger : MonoBehaviour
{
    [Header("Riferimento al Camino")]
    public ECAFireplace fireplace;

    [Header("Configurazione Input")]
    public OVRInput.Button buttonToPress = OVRInput.Button.One;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    void Update()
    {
        if (OVRInput.GetDown(buttonToPress, controller))
        {
            if (fireplace != null)
            {
                if (fireplace.isLit == ECABoolean.FALSE)
                {
                    Debug.Log("[Oculus] Tasto premuto. Accendo il camino...");
                    
                    Action fireplaceActionSignal = new Action(fireplace.gameObject, "ignite");
                
                    RuleEngine.GetInstance().ExecuteAction(fireplaceActionSignal);
                
                    Debug.Log("[Oculus] Segnale 'ignite' inviato al RuleEngine via ExecuteAction!");
                }
                else
                {
                    Debug.Log("[Oculus] Tasto premuto. Spengo il camino...");
                    
                    Action fireplaceActionSignal = new Action(fireplace.gameObject, "extinguish");
                
                    RuleEngine.GetInstance().ExecuteAction(fireplaceActionSignal);
                
                    Debug.Log("[Oculus] Segnale 'extinguish' inviato al RuleEngine via ExecuteAction!");
                }

            }
        }
    }
}