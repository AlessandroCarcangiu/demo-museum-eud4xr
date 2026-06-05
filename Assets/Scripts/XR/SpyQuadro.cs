using UnityEngine;
using ECARules4All_DLL;

public class SpyQuadro : MonoBehaviour
{
    // Dentro SpyQuadro.cs
    void OnTriggerEnter(Collider other)
    {
        // Otteniamo il percorso completo dell'oggetto che ha attivato il trigger
        string fullPath = other.name;
        Transform current = other.transform;
        while (current.parent != null)
        {
            current = current.parent;
            fullPath = current.name + "/" + fullPath;
        }

        Debug.Log($"<color=yellow><b>[SPY QUADRO] Collisione Rilevata!</b></color>\n" +
                  $"• Nome Oggetto: {other.name}\n" +
                  $"• Percorso Hierarchy: {fullPath}\n" +
                  $"• Tag: {other.tag} | Layer: {LayerMask.LayerToName(other.gameObject.layer)}\n" +
                  $"• Ha ECAObject? {other.GetComponentInParent<ECARules4All_DLL.ECAObject>() != null}");
    }
    
}