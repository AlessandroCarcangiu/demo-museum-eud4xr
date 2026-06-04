using Serilog;
using UnityEngine;

namespace MRTaxonomyExtraScripts
{
    public class CleanDustFloor: MonoBehaviour
    {
        public Texture newTexture;
        
        public void SetCleanTexture()
        {
            Log.Information("[CleanDustFloor] Start SetCleanTexture");
            Renderer rend = GetComponentInChildren<Renderer>();
            Material mat = rend.material;
            mat.mainTexture = newTexture;
            Log.Information("[CleanDustFloor] End SetCleanTexture");
        }
    }
}