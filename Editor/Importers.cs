using UnityEditor.AssetImporters;
using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Editor
#pragma warning restore IDE0130 // Namespace does not match folder structure
{

    [ScriptedImporter(1, new[] { "urta", "urt3d", "urt3da" })]
    public class ImporterAsset : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var go = new GameObject();
            var comp = go.AddComponent<Wrapper>();
            comp.PathRaw = ctx.assetPath;

            ctx.AddObjectToAsset("Main Object", go);
            ctx.SetMainObject(go);
        }
    }
}
