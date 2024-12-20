using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace SVP.Editor.Events
{
    using static UnityEventDrawerUtilities;
    using static UnityEventUtilities;

    [CustomPropertyDrawer(typeof(UnityEventBase), true)]
    public class UnityEventDrawer : UnityEditorInternal.UnityEventDrawer
    {
        private const string INVOKE_METHOD = "Invoke";
        private const string EMPTY_OPTION_TEXT = "No Function";

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
            listView.makeItem = () =>
            {
                return CreateUnityEventItem();
            };

            listView.bindItem = delegate (VisualElement element, int i)
            {
                SerializedProperty callsProperty = eventProperty.FindPropertyRelative(CALLS_PROPERTY);
                if (i >= callsProperty.arraySize)
                {
                    Debug.LogError($"Trying to bind an item to the list of which the index {i} is larger than the array size on event {eventProperty.displayName}");
                    return;
                }

                SerializedProperty listenerProperty = callsProperty.GetArrayElementAtIndex(i);
                object propertyData = CreatePropertyDataForListener(listenerProperty);

                Func<GenericMenu> createMenuCallback = () => BuildPopupList(listenerProperty.FindPropertyRelative(TARGET_PROPERTY).objectReferenceValue, this.GetDummyEventFieldValue(), listenerProperty);
                Func<string, string> formatSelectedValueCallback = (string value) => this.GetFunctionDropdownText(listenerProperty);
                Func<SerializedProperty> getArgumentCallback = () => this.GetArgument(listenerProperty);

                element.ItemBindFields(propertyData, createMenuCallback, formatSelectedValueCallback, getArgumentCallback);
            };
        }

        #endregion List

        #region GUI

        protected override void DrawEvent(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y++;
            
            SerializedProperty listener = this.GetListenersArray().GetArrayElementAtIndex(index);
            SerializedProperty target = listener.GetTargetProperty();
            SerializedProperty methodName = listener.GetMethodNameProperty();
            Rect[] rowRects = this.GetRowRects(rect);

            Color originalBackgroundColour = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            EditorGUI.PropertyField(rowRects[0], listener.GetCallStateProperty(), GUIContent.none);

            EditorGUI.BeginChangeCheck();
            GUI.Box(rowRects[1], GUIContent.none);
            EditorGUI.PropertyField(rowRects[1], target, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                methodName.stringValue = null;
            }

            SerializedProperty argument = this.GetArgument(listener);
            PersistentListenerMode listenerMode = GetMode(listener.GetModeProperty());
            if (target.objectReferenceValue == null || string.IsNullOrEmpty(methodName.stringValue))
            {
                listenerMode = PersistentListenerMode.Void;
            }

            if (listenerMode == PersistentListenerMode.Object)
            {
                string objectArgumentTypeName = listener.GetArgumentsProperty().GetObjectArgumentProperty().stringValue;
                Type objectArgumentType = typeof(UnityEngine.Object);
                if (!string.IsNullOrEmpty(objectArgumentTypeName))
                {
                    objectArgumentType = Type.GetType(objectArgumentTypeName, false) ?? typeof(UnityEngine.Object);
                }

                EditorGUI.BeginChangeCheck();
                UnityEngine.Object objectValue = EditorGUI.ObjectField(rowRects[3], GUIContent.none, argument.objectReferenceValue, objectArgumentType, true);
                if (EditorGUI.EndChangeCheck())
                {
                    argument.objectReferenceValue = objectValue;
                }
            }
            else if (listenerMode != PersistentListenerMode.Void && listenerMode != 0)
            {
                EditorGUI.PropertyField(rowRects[3], argument, GUIContent.none);
            }

            using (new EditorGUI.DisabledScope(target.objectReferenceValue == null))
            {
                EditorGUI.BeginProperty(rowRects[2], GUIContent.none, methodName);
                GUIContent dropdownContent;
                if (EditorGUI.showMixedValue)
                {
                    dropdownContent = new GUIContent("-", "Mixed Values");
                }
                else
                {
                    string functionDropdownText = this.GetFunctionDropdownText(listener);
                    dropdownContent = new GUIContent(functionDropdownText.ToString());
                }

                if (EditorGUI.DropdownButton(rowRects[2], dropdownContent, FocusType.Passive, EditorStyles.popup))
                {
                    BuildPopupList(target.objectReferenceValue, GetDummyEvent(listener), listener).DropDown(rowRects[2]);
                }
                EditorGUI.EndProperty();
            }

            GUI.backgroundColor = originalBackgroundColour;
        }

        #endregion GUI

        #region Popup List

        internal static GenericMenu BuildPopupList(UnityEngine.Object target, UnityEventBase dummyEvent, SerializedProperty listenerProperty)
        {
            if (target is Component)
            {
                target = (target as Component).gameObject;
            }

            GenericMenu menu = new GenericMenu();

            // We add the empty option
            SerializedProperty methodName = listenerProperty.GetMethodNameProperty();
            bool unassignedMethod = string.IsNullOrEmpty(methodName.stringValue);

            menu.AddItem(new GUIContent(EMPTY_OPTION_TEXT), unassignedMethod, ClearEventFunction, CreateEmptyUnityEventFunction(listenerProperty));

            if (target == null)
            {
                return menu;
            }

            menu.AddSeparator("");

            Type eventType = dummyEvent.GetType();
            System.Reflection.MethodInfo invokeMethod = eventType.GetMethod(INVOKE_METHOD);

            List<Type> delegateArgumentsTypes = new List<Type>();
            foreach (System.Reflection.ParameterInfo paramInfo in invokeMethod.GetParameters())
            {
                delegateArgumentsTypes.Add(paramInfo.ParameterType);
            }

            GeneratePopUpForType(menu, target, target.GetType().Name, listenerProperty, delegateArgumentsTypes);

            // Might a scriptable object and those don't have components so we can return here
            if (target is not GameObject)
            {
                return menu;
            }

            Dictionary<string, int> typeCount = CollectionPool<Dictionary<string, int>, KeyValuePair<string, int>>.Get();

            Component[] components = (target as GameObject).GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    continue;
                }

                Type componentType = component.GetType();
                string targetName = componentType.Name;
                int count = 0;
                if (!typeCount.TryGetValue(targetName, out count))
                {
                    targetName = $"{targetName} ({count})";
                }

                GeneratePopUpForType(menu, component, targetName, listenerProperty, delegateArgumentsTypes);

                typeCount[componentType.Name] = count + 1;
            }

            CollectionPool<Dictionary<string, int>, KeyValuePair<string, int>>.Release(typeCount);
            return menu;
        }

        internal static void GeneratePopUpForType(GenericMenu menu, UnityEngine.Object target, string targetName, SerializedProperty listener, List<Type> delegateArgumentsTypes)
        {
            List<System.Reflection.MethodInfo> eventMethods = EventAttributeHelper.GetAllEventAttributeMethodsForType(target.GetType());

            HashSet<string> uniqueGroups = new HashSet<string>();
            bool hasUngroupedEventMethods = false;
            foreach (System.Reflection.MethodInfo eventMethod in eventMethods)
            {
                string eventMethodGroup = EventAttributeHelper.GetEventAttributeForMethod(eventMethod).Group;
                if (!string.IsNullOrEmpty(eventMethodGroup))
                {
                    uniqueGroups.Add(eventMethodGroup);
                }
                else
                {
                    hasUngroupedEventMethods = true;
                }
            }

            if (uniqueGroups.Count > 0)
            {
                menu.AddDisabledItem(new GUIContent(targetName + "/Groups"));
            }

            foreach (string group in uniqueGroups)
            {
                string groupTargetName = $"{targetName}/{group}";
                GeneratePopUpForGroupOfType(menu, target, targetName, listener, delegateArgumentsTypes, group);
            }

            if (hasUngroupedEventMethods)
            {
                menu.AddItem(new GUIContent($"{targetName}/ "), false, null);
                GeneratePopUpForGroupOfType(menu, target, targetName, listener, delegateArgumentsTypes, string.Empty);
            }

            if (EventAttributeHelper.HasMethodsWithoutEventAttribute(target.GetType()))
            {
                string otherGroupName = "Other";
                if (uniqueGroups.Count == 0 && !hasUngroupedEventMethods)
                {
                    otherGroupName = string.Empty;
                }

                menu.AddItem(new GUIContent($"{targetName}/ "), false, null);
                GeneratePopUpForNonEventOfType(menu, target, targetName, listener, delegateArgumentsTypes, otherGroupName);
            }
        }

        private static void GeneratePopUpForGroupOfType(GenericMenu menu, UnityEngine.Object target, string targetName, SerializedProperty listener, List<Type> delegateArgumentsTypes, string group)
        {
            string groupText = !string.IsNullOrEmpty(group) ? $"/{group} " : "";

            IList validMethodMapList = CreateValidMethodMapList();

            if (delegateArgumentsTypes.Count != 0)
            {
                GetMethodsForTargetAndMode(target, delegateArgumentsTypes.ToArray(), validMethodMapList, PersistentListenerMode.EventDefined);
                if (validMethodMapList.Count > 0)
                {
                    SanitizeListForEventGroup(validMethodMapList, group);

                    SortEventMethodList(validMethodMapList);

                    if (validMethodMapList.Count > 0)
                    {
                        string argumentTypeNames = string.Join(", ", delegateArgumentsTypes.Select((Type e) => GetTypeName(e)).ToArray());
                        menu.AddDisabledItem(new GUIContent($"{targetName}{groupText}/Dynamic {argumentTypeNames}"));
                        
                        AddMethodsToMenuForGroup(menu, listener, validMethodMapList, targetName, group);
                    }
                }
            }

            validMethodMapList.Clear();
            GetMethodsForTargetAndMode(target, new Type[1] {typeof(float)}, validMethodMapList, PersistentListenerMode.Float);
            GetMethodsForTargetAndMode(target, new Type[1] {typeof(int)}, validMethodMapList, PersistentListenerMode.Int);
            GetMethodsForTargetAndMode(target, new Type[1] {typeof(string)}, validMethodMapList, PersistentListenerMode.String);
            GetMethodsForTargetAndMode(target, new Type[1] {typeof(bool)}, validMethodMapList, PersistentListenerMode.Bool);
            GetMethodsForTargetAndMode(target, new Type[1] {typeof(UnityEngine.Object)}, validMethodMapList, PersistentListenerMode.Object);
            GetMethodsForTargetAndMode(target, new Type[0], validMethodMapList, PersistentListenerMode.Void);

            if (validMethodMapList.Count > 0)
            {
                SanitizeListForEventGroup(validMethodMapList, group);

                SortEventMethodList(validMethodMapList);

                if (validMethodMapList.Count > 0)
                {
                    if (delegateArgumentsTypes.Count != 0)
                    {
                        menu.AddDisabledItem(new GUIContent($"{targetName}{groupText}/Static Parameters"));
                    }

                    AddMethodsToMenuForGroup(menu, listener, validMethodMapList, targetName, group);
                }
            }
        }

        private static void GeneratePopUpForNonEventOfType(GenericMenu menu, UnityEngine.Object target, string targetName, SerializedProperty listener, List<Type> delegateArgumentsTypes, string group)
        {
            string groupText = !string.IsNullOrEmpty(group) ? $"/{group} " : "";

            IList validMethodMapList = CreateValidMethodMapList();

            if (delegateArgumentsTypes.Count != 0)
            {
                GetMethodsForTargetAndMode(target, delegateArgumentsTypes.ToArray(), validMethodMapList, PersistentListenerMode.EventDefined);
                if (validMethodMapList.Count > 0)
                {
                    SanitizeListFromEventGroup(validMethodMapList);

                    if (validMethodMapList.Count > 0)
                    {
                        string argumentTypeNames = string.Join(", ", delegateArgumentsTypes.Select((Type e) => GetTypeName(e)).ToArray());
                        menu.AddDisabledItem(new GUIContent($"{targetName}{groupText}/Dynamic {argumentTypeNames}"));
                        AddMethodsToMenu(menu, listener, validMethodMapList, $"{targetName}{groupText}");
                    }
                }
            }

            validMethodMapList.Clear();
            GetMethodsForTargetAndMode(target, new Type[1] { typeof(float) }, validMethodMapList, PersistentListenerMode.Float);
            GetMethodsForTargetAndMode(target, new Type[1] { typeof(int) }, validMethodMapList, PersistentListenerMode.Int);
            GetMethodsForTargetAndMode(target, new Type[1] { typeof(string) }, validMethodMapList, PersistentListenerMode.String);
            GetMethodsForTargetAndMode(target, new Type[1] { typeof(bool) }, validMethodMapList, PersistentListenerMode.Bool);
            GetMethodsForTargetAndMode(target, new Type[1] { typeof(UnityEngine.Object) }, validMethodMapList, PersistentListenerMode.Object);
            GetMethodsForTargetAndMode(target, new Type[0], validMethodMapList, PersistentListenerMode.Void);

            if (validMethodMapList.Count > 0)
            {
                SanitizeListFromEventGroup(validMethodMapList);

                if (validMethodMapList.Count > 0)
                {
                    if (delegateArgumentsTypes.Count != 0)
                    {
                        menu.AddDisabledItem(new GUIContent($"{targetName}{groupText}/Static Parameters"));
                    }

                    AddMethodsToMenu(menu, listener, validMethodMapList, $"{targetName}{groupText}");
                }
            }
        }

        private static bool AddMethodsToMenuForGroup(GenericMenu menu, SerializedProperty listener, IList methods, string targetName, string group)
        {
            List<object> methodMaps = new List<object>();
            List<string> targetNames = new List<string>();

            foreach (object validMethodMap in methods)
            {
                SVP.Events.EventAttribute eventAttribute = GetEventAttributeFromValidMethodMapMethodField(validMethodMap);
                if (eventAttribute != null && group == eventAttribute.Group)
                {
                    string newTargetName = string.IsNullOrEmpty(group) ? targetName : $"{targetName}/{group}";
                    targetNames.Add(newTargetName);
                    methodMaps.Add(validMethodMap);
                }
            }

            if (methodMaps.Count == 0)
            {
                return false;
            }

            for (int i = 0, count = methodMaps.Count; i < count; i++)
            {
                AddFunctionsForScript(menu, listener, methodMaps[i], targetNames[i]);
            }

            return true;
        }

        private static void AddMethodsToMenu(GenericMenu menu ,SerializedProperty listener, IList methods, string targetName)
        {
            for (int i = 0, count = methods.Count; i < count; i++)
            {
                AddFunctionsForScript(menu, listener, methods[i], targetName);
            }
        }

        #endregion Popup List
    }
}