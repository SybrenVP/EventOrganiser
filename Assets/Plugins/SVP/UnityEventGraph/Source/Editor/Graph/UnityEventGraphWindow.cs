using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    public class UnityEventGraphWindow : EditorWindow
    {
        private UnityEventGraphWindowSettings _settings = null;
        private UnityEventGraphView _eventGraphView = null;

        [MenuItem("Window/SVP/UnityEventWindow")]
        public static void OpenWindow()
        {
            UnityEventGraphWindow window = GetWindow<UnityEventGraphWindow>();
            window.titleContent = new GUIContent("UnityEventGraphWindow");
            window.minSize = new Vector2(800f, 600f);
        }

        public void CreateGUI()
        {
            _settings = UnityEventGraphWindowSettings.GetOrCreateSettings();

            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTreeXml = _settings.UnityEventWindowXml;
            visualTreeXml.CloneTree(root);

            StyleSheet styleSheet = _settings.UnityEventWindowStyleSheet;
            root.styleSheets.Add(styleSheet);
            
            _eventGraphView = root.Q<UnityEventGraphView>();
        }
    }
}