using UnityEditor;
using UnityEngine;

namespace Petit.Script
{
    [CustomEditor(typeof(PetitScript))]
    public class PetitScriptEditor : Editor
    {
        public void OnEnable()
        {
            _code = serializedObject.FindProperty(nameof(PetitScript.Code));
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Script Code");
            _code.stringValue = EditorGUILayout.TextArea(_code.stringValue, GUILayout.ExpandHeight(true));
            serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty _code;
    }
}
