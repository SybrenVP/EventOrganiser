using UnityEditor;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace SVP.Editor.Events
{
    public class SceneEventGraphData : ScriptableObject
    {
        private const string DEFAULT_PATH = "Assets/Plugins/SVP/UnityEventGraph/Data/Scenes";

        [SerializeField] private string _sceneGuid = string.Empty;

        // TODO: List of all nodes
        // Event owner nodes are like Start nodes, they're always present in the graph and you can only have one of them. 


        public static SceneEventGraphData GetOrCreateSceneGraph(GUID sceneGuid)
        {
            SceneEventGraphData sceneGraphData = FindSceneGraph(sceneGuid.ToString());

            if (sceneGraphData == null)
            {
                sceneGraphData = CreateSceneGraph(sceneGuid.ToString());
            }

            return sceneGraphData;
        }

        private static SceneEventGraphData FindSceneGraph(string sceneGuid)
        {
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(SceneEventGraphData).Name}");

            if (guids.Length == 0)
            {
                return null;
            }

            SceneEventGraphData sceneEventGraphData = null;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneEventGraphData sceneGraph = AssetDatabase.LoadAssetAtPath<SceneEventGraphData>(path);
                if (sceneGraph._sceneGuid == sceneGuid)
                {
                    sceneEventGraphData = sceneGraph;
                    break;
                }
            }

            return sceneEventGraphData;
        }

        private static SceneEventGraphData CreateSceneGraph(string sceneGuid)
        {
            SceneEventGraphData sceneGraph = ScriptableObject.CreateInstance<SceneEventGraphData>();
            AssetDatabase.CreateAsset(sceneGraph, $"{DEFAULT_PATH}/{sceneGuid}.asset");
            AssetDatabase.SaveAssets();

            return sceneGraph;
        }
    }
}