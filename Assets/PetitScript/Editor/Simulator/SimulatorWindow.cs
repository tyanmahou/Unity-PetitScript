using Petit.Script;
using UnityEditor;
using UnityEngine;

namespace Petit
{
    class SimulatorWindow : EditorWindow
    {
        [MenuItem("Window/PetitScript/Simulator")]
        public static void ShowWindow()
        {
            var window = GetWindow<SimulatorWindow>("Simulator");
        }

        void OnGUI()
        {
            _scriptFile = EditorGUILayout.ObjectField("File", _scriptFile, typeof(PetitScript), false) as PetitScript;
            DrawSeparator();
            using (var contentScroller = new GUILayout.ScrollViewScope(_position))
            {
                if (_scriptFile != null)
                {
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        EditorGUILayout.TextArea(_scriptFile.Code, GUILayout.ExpandHeight(true));
                    }
                }
                else
                {
                    _scriptCode = EditorGUILayout.TextArea(_scriptCode, GUILayout.ExpandHeight(true));
                }
            }
            DrawSeparator();
            if (GUILayout.Button("Run"))
            {
                var code = (_scriptFile != null) ? _scriptFile.Code : _scriptCode;
                var interpreta = new Core.Interpreter();
                _result = interpreta.Run(code).ToString();
            }
            EditorGUILayout.LabelField("output", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField(_result);
            }
        }
        static void DrawSeparator()
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);
            EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }
        PetitScript _scriptFile;
        string _scriptCode;
        string _result = string.Empty;
        Vector2 _position;
    }
}
