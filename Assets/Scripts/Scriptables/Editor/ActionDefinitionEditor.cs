#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using Core.Combat.Actions;
using Scriptables;

[CustomEditor(typeof(ActionDefinition))]
public class ActionDefinitionEditor : Editor
{
    private string[] availableActions;
    private string[] displayedActionNames;
    private int selectedIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var def = (ActionDefinition)target;

        // Select all BattleAction classes with the ActionClassAttribute attribute
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(BattleAction).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<ActionClassAttribute>()
            })
            .Where(x => x.Attr != null)
            .ToList();

        availableActions = types.Select(t => t.Attr.ActionName).ToArray();
        displayedActionNames = types.Select(t => t.Type.Name).ToArray();

        selectedIndex = Mathf.Max(0, Array.IndexOf(availableActions, def.actionClassName));
        if (selectedIndex < 0) selectedIndex = 0;
        
        if (!availableActions.Contains(def.actionClassName))
        {
            Debug.LogWarning($"Action class name '{def.actionClassName}' not found in available types. Resetting to first.");
        }

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("Action Class", selectedIndex, displayedActionNames);
        if (EditorGUI.EndChangeCheck())
        {
            def.actionClassName = availableActions[selectedIndex];
            EditorUtility.SetDirty(def);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif