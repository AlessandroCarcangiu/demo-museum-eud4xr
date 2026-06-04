using Serilog;
using UnityEngine;

namespace MRTaxonomyExtraScripts
{
    public class CleanStainFloor: MonoBehaviour
    {
        public Texture newTexture;
        
        public void SetCleanTexture()
        {
            Log.Information("[CleanStainFloor] Start SetCleanTexture");
            Renderer rend = GetComponentInChildren<Renderer>();
            Material mat = rend.material;
            mat.mainTexture = newTexture;
            Log.Information("[CleanStainFloor] End SetCleanTexture");
        }
    }
}