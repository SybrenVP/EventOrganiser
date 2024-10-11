using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{

    using static UnityEventDrawerUtilities;
    using static UnityEventUtilities;

    [CustomPropertyDrawer(typeof(UnityEventBase), true)]
    public class UnityEventDrawer : UnityEditorInternal.UnityEventDrawer
    {
        #region Visual Element

        // Class Names
        private const string _unityEventContainerClassName = "unity-event__container";
        private const string _unityButtonClassName = "unity-button";
        private const string _unityListViewHeaderClassName = "unity-list-view__header";

        // Foldout
        private const float _headerFoldoutMarginLeft = 0f;
        private const float _headerFoldoutMarginRight = 5f;
        private Image _headerFoldoutIcon = null;
        private Texture2D _headerFoldoutHiddenTexture = null;
        private Texture2D _headerFoldoutShownTexture = null;
        private const string _headerFoldoutHiddenIconName = "d_forward";
        private const string _headerFoldoutShownIconName = "d_icon dropdown";

        // ListView
        private ListView _listenerList = null;

        public override VisualElement CreatePropertyGUI(SerializedProperty eventProperty)
        {
            // Parent visual element
            VisualElement visualElement = new VisualElement();
            visualElement.AddToClassList(_unityEventContainerClassName);

            // Horizontal header
            VisualElement header = new VisualElement();
            header.AddToClassList(_unityListViewHeaderClassName);
            header.style.flexDirection = FlexDirection.Row;
            visualElement.Add(header);

            // Foldout button for decreasing Unity Event inspector size
            Button foldout = new Button(ToggleList);
            foldout.RemoveFromClassList(_unityButtonClassName);
            foldout.style.marginLeft = _headerFoldoutMarginLeft;
            foldout.style.marginRight = _headerFoldoutMarginRight;
            foldout.style.alignSelf = Align.Center;
            header.Add(foldout);

            // Foldout icon
            _headerFoldoutIcon = new Image();
            _headerFoldoutHiddenTexture = (Texture2D)EditorGUIUtility.IconContent(_headerFoldoutHiddenIconName).image;
            _headerFoldoutShownTexture = (Texture2D)EditorGUIUtility.IconContent(_headerFoldoutShownIconName).image;
            _headerFoldoutIcon.style.backgroundImage = _headerFoldoutHiddenTexture;
            _headerFoldoutIcon.style.width = _headerFoldoutHiddenTexture.width;
            _headerFoldoutIcon.style.height = _headerFoldoutHiddenTexture.height;
            foldout.Add(_headerFoldoutIcon);

            // Event name label
            Label label = new Label();
            label.text = eventProperty.displayName;
            label.tooltip = eventProperty.tooltip;
            label.style.flexGrow = 1;
            header.Add(label);

            this.SetDummyEventFieldValue(GetDummyEvent(eventProperty));

            // Create the listener list view from the base class
            _listenerList = this.CreateListView(eventProperty);
            if (_listenerList != null)
            {
                // and override the list view if it exists
                OverrideListView(ref _listenerList, eventProperty);
                _listenerList.style.display = DisplayStyle.None;
                visualElement.Add(_listenerList);
            }

            // Add a right click menu from the base class
            RegisterRightClickMenu(label, eventProperty);

            return visualElement;
        }

        #endregion Visual Element

        #region Foldout Behaviour

        private void ToggleList()
        {
            if (_listenerList.style.display == DisplayStyle.Flex)
            {
                _listenerList.style.display = DisplayStyle.None;
                _headerFoldoutIcon.style.backgroundImage = _headerFoldoutHiddenTexture;
            }
            else
            {
                _listenerList.style.display = DisplayStyle.Flex;
                _headerFoldoutIcon.style.backgroundImage = _headerFoldoutShownTexture;
            }
        }

        #endregion Foldout Behaviour

        #region List

        private void OverrideListView(ref ListView listView, SerializedProperty eventProperty)
        {
            listView.makeItem = () => new UnityEventItem();

            listView.bindItem = delegate (VisualElement element, int i)
            {
                SerializedProperty callsProperty = eventProperty.FindPropertyRelative(CALLS_PROPERTY);
                if (i >= callsProperty.arraySize)
                {
                    Debug.LogError($"Trying to bind an item to the list of which the index {i} is larger than the array size on event {eventProperty.displayName}");
                    return;
                }

                SerializedProperty listenerProperty = callsProperty.GetArrayElementAtIndex(i);
                PropertyData propertyData = CreatePropertyDataForListener(listenerProperty);

                Func<GenericMenu> createMenuCallback = () => BuildPopupList(propertyData.ListenerTarget.objectReferenceValue, this.GetDummyEventFieldValue(), listenerProperty);
                Func<string, string> formatSelectedValueCallback = (string value) => this.GetFunctionDropdownText(listenerProperty);
                Func<SerializedProperty> getArgumentCallback = () => this.GetArgument(listenerProperty);

                UnityEventItem unityEventItem = element as UnityEventItem;
                unityEventItem.BindFields(propertyData, createMenuCallback, formatSelectedValueCallback, getArgumentCallback);
            };
        }

        #endregion

        #region Popup List

        internal static GenericMenu BuildPopupList(UnityEngine.Object target, UnityEventBase dummyEvent, SerializedProperty listenerProperty)
        {
            return null;
        }

        #endregion Popup List
    }
}