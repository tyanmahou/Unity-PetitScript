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
                var interpreter = new Core.Interpreter();
                interpreter
                    .SetOnResult((string v) =>
                    {
                        _result = v;
                        _error = false;
                    })
                    .SetOnSyntaxError(e => {
                        _result = e;
                        _error = true;
                    })
                    .Run(code)
                    ;
            }
            EditorGUILayout.LabelField("output", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                var color = GUI.color;
                if (_error)
                {
                    GUI.color = Color.red;
                }
                {
                    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                    labelStyle.wordWrap = true;
                    labelStyle.richText = true;
                    var height = labelStyle.CalcHeight(new GUIContent(_result), EditorGUIUtility.currentViewWidth);
                    EditorGUILayout.SelectableLabel(_result, labelStyle, GUILayout.Height(height));
                }
                GUI.color = color;
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
        bool _error = false;
        Vector2 _position;
    }
}
