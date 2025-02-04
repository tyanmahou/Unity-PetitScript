using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Petit.Script
{
    [ScriptedImporter(1, "petit")]
    public class PetitScriptImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var text = File.ReadAllText(ctx.assetPath);

            PetitScript petitScript = ScriptableObject.CreateInstance<PetitScript>();
            petitScript.Code = text;

            ctx.AddObjectToAsset("Main", petitScript);
            ctx.SetMainObject(petitScript);
        }
    }
}
