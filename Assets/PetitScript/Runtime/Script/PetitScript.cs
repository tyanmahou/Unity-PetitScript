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
    }
}
