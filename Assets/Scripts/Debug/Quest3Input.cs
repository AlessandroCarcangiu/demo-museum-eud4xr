using UnityEngine.InputSystem;

namespace MixedReality.Toolkit.UX.Museum.Scripts.Debug
{
    public class Quest3Input
    {
        void OnEnable()
        {
            InputSystem.onActionChange += OnActionTriggered;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= OnActionTriggered;
        }

        private void OnActionTriggered(object actionOrControl, InputActionChange change)
        {
            // Intercettiamo solo il momento esatto in cui il tasto viene affondato (Pressed)
            if (change == InputActionChange.ActionStarted)
            {
                InputAction action = actionOrControl as InputAction;
                if (action != null && action.activeControl != null)
                {
                    string controlPath = action.activeControl.path.ToLower();

                    // Filtro di sicurezza: tracciamo SOLO se l'input arriva da un controller VR (Left/Right Hand)
                    // Escludiamo i vettori continui di posizione e rotazione
                    if ((controlPath.Contains("hand") || controlPath.Contains("xr")) && 
                        !controlPath.Contains("position") && 
                        !controlPath.Contains("rotation") && 
                        !controlPath.Contains("velocity"))
                    {
                        string mano = controlPath.Contains("lefthand") ? "<color=yellow>MANO SINISTRA</color>" : "<color=cyan>MANO DESTRA</color>";
                    
                        UnityEngine.Debug.Log($"<color=lime><b>[QUEST 3 INPUT]</b></color> Rilevato su {mano}\n" +
                                  $"• <b>Nome Azione (MRTK):</b> <color=white>{action.name}</color>\n" +
                                  $"• <b>Tasto Fisico Hardware:</b> {action.activeControl.name}\n" +
                                  $"• <b>Percorso completo per lo script:</b> <color=orange>{action.activeControl.path}</color>\n");
                    }
                }
            }
        }
    }
}