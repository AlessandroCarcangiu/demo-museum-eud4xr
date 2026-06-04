using Serilog;
using UnityEngine;

namespace MRTaxonomyExtraScripts
{
    public class CleanStainTable: MonoBehaviour
    {
        public Texture newTexture;
        
        public void SetCleanTexture()
        {
            Log.Information("[CleanStainTable] Start SetCleanTexture");
            Renderer rend = GetComponentInChildren<Renderer>();
            Material mat = rend.material;
            mat.mainTexture = newTexture;
            Log.Information("[CleanStainTable] End SetCleanTexture");
        }
    }
}