using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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

            PopulateEventGraphViewForScene(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path));
        }

        private void PopulateEventGraphViewForScene(GUID sceneGuid)
        {
            SceneEventGraphData sceneGraph = SceneEventGraphData.GetOrCreateSceneGraph(sceneGuid);
            _eventGraphView.Populate(sceneGraph);

            EditorApplication.delayCall += () =>
            {
                _eventGraphView.FrameAll();
            };
        }
    }
}