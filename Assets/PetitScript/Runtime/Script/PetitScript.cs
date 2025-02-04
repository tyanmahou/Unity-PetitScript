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

        public Value Run(Variables variables = null)
        {
            var interpreter = new Interpreter();
            if (variables != null)
            {
                interpreter.Variables = variables;
            }
            return interpreter.Run(Code);
        }
    }
}
