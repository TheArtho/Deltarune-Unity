#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using Core.Combat.Actions;
using UnityEditor;
using UnityEngine;

namespace Scriptables.Editor
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : UnityEditor.Editor
    {
        private string[] availableItems;
        private string[] displayedItemNames;
        private int selectedIndex;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var def = (ItemDefinition)target;
            
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(BattleItem).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ItemClassAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToList();
            
            if (types.Count == 0)
            {
                return;
            }

            availableItems = types.Select(t => t.Attr.ClassName).ToArray();
            displayedItemNames = types.Select(t => t.Type.Name).ToArray();

            selectedIndex = Mathf.Max(0, Array.IndexOf(availableItems, def.itemClassName));
            if (def.itemClassName == "")
            {
                def.itemClassName = availableItems[selectedIndex];
            }

            if (!availableItems.Contains(def.itemClassName))
            {
                Debug.LogWarning($"Item class name '{def.itemClassName}' not found in available types. Resetting to first.");
            }

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup("Item Class", selectedIndex, displayedItemNames);
            if (EditorGUI.EndChangeCheck())
            {
                def.itemClassName = availableItems[selectedIndex];
                EditorUtility.SetDirty(def);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
