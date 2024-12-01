using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    public class UnityEventGraphWindowSettings : ScriptableObject
    {
        private const string DEFAULT_PATH = "Assets/Plugins/SVP/UnityEventGraph/Data/UnityEventGraphWindowSettings.asset";

        [SerializeField] private VisualTreeAsset _unityEventWindowXml = null;
        public VisualTreeAsset UnityEventWindowXml
        {
            get { return _unityEventWindowXml; }
        }

        [SerializeField] private StyleSheet _unityEventWindowStyleSheet = null;
        public StyleSheet UnityEventWindowStyleSheet
        {
            get { return _unityEventWindowStyleSheet; }
        }

        internal static UnityEventGraphWindowSettings GetOrCreateSettings()
        {
            UnityEventGraphWindowSettings settings = FindSettings();

            if (settings == null)
            {
                settings = CreateSettings();
            }

            return settings;
        }

        private static UnityEventGraphWindowSettings FindSettings()
        {
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(UnityEventGraphWindowSettings).Name}");
            if (guids.Length == 0)
            {
                return null;
            }

            if (guids.Length > 1)
            {
                Debug.LogWarning($"Found multiple ({guids.Length}) graph window settings files, using the first");
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<UnityEventGraphWindowSettings>(path);
        }

        private static UnityEventGraphWindowSettings CreateSettings()
        {
            UnityEventGraphWindowSettings settings = ScriptableObject.CreateInstance<UnityEventGraphWindowSettings>();
            AssetDatabase.CreateAsset(settings, DEFAULT_PATH);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}