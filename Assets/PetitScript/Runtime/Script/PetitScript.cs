using UnityEngine;

namespace Petit.Script
{
    /// <summary>
    /// インタプリタ
    /// </summary>
    [CreateAssetMenu(fileName = "NewPetitScript", menuName = "PetitScript/NewScript")]
    public class PetitScript : ScriptableObject
    {
        public string Code;

        public Core.Value Run(Core.Variables variables = null)
        {
            var interpreter = new Core.Interpreter();
            if (variables != null)
            {
                interpreter.Variables = variables;
            }
            return interpreter.Run(Code);
        }
    }
}
