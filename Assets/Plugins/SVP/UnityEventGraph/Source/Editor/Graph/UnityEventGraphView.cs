using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    public class UnityEventGraphView : GraphView
    {
        private UnityEventGraphWindowSettings _settings = null;

        public new class UxmlFactory : UxmlFactory<UnityEventGraphView, GraphView.UxmlTraits> { }

        public UnityEventGraphView()
        {
            _settings = UnityEventGraphWindowSettings.GetOrCreateSettings();

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new DoubleClickGameObjectSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            StyleSheet styleSheet = _settings.UnityEventWindowStyleSheet;
            styleSheets.Add(styleSheet);

            // TODO: Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void Populate(SceneEventGraphData sceneGraphData)
        {

        }
    }
}