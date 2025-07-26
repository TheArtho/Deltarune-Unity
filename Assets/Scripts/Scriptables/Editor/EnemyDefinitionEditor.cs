#if UNITY_EDITOR
using System;
using System.Linq;
using Core.Combat;
using UnityEditor;
using UnityEngine;

namespace Scriptables.Editor
{
    [CustomEditor(typeof(EnemyDefinition))]
    public class EnemyDefinitionEditor : UnityEditor.Editor
    {
        private string[] availableActions;
        private string[] displayedActionNames;
        private int selectedIndex;

        private SerializedProperty nameProp;
        private SerializedProperty hpProp;
        private SerializedProperty attackProp;
        private SerializedProperty defenseProp;
        private SerializedProperty haveMercyProp;
        private SerializedProperty battleSpritePrefabProp;
        private SerializedProperty playerActionsProp;
        private SerializedProperty bulletPatternsProp;
        private SerializedProperty enemyClassNameProp;

        private void OnEnable()
        {
            nameProp = serializedObject.FindProperty("name");
            hpProp = serializedObject.FindProperty("hp");
            attackProp = serializedObject.FindProperty("attack");
            defenseProp = serializedObject.FindProperty("defense");
            haveMercyProp = serializedObject.FindProperty("haveMercy");
            battleSpritePrefabProp = serializedObject.FindProperty("battleSpritePrefab");
            playerActionsProp = serializedObject.FindProperty("playerActions");
            enemyClassNameProp = serializedObject.FindProperty("enemyClassName");
            bulletPatternsProp = serializedObject.FindProperty("bulletPatterns");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var def = (EnemyDefinition)target;

            // --- Behavior Class Selector ---
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Enemy).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t => t.GetCustomAttributes(typeof(EnemyClassAttribute), false).Length > 0)
                .ToList();
            
            if (types.Count == 0) return;

            availableActions = types.Select(t => t.AssemblyQualifiedName).ToArray();
            displayedActionNames = types.Select(t => t.Name).ToArray();

            selectedIndex = Mathf.Max(0, Array.IndexOf(availableActions, enemyClassNameProp.stringValue));
            selectedIndex = EditorGUILayout.Popup("Enemy Logic Class", selectedIndex, displayedActionNames);
            enemyClassNameProp.stringValue = availableActions[selectedIndex];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Enemy Parameters", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(nameProp);
            EditorGUILayout.PropertyField(hpProp);
            EditorGUILayout.PropertyField(attackProp);
            EditorGUILayout.PropertyField(defenseProp);
            EditorGUILayout.PropertyField(haveMercyProp);
            EditorGUILayout.PropertyField(battleSpritePrefabProp);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Each list represents actions available to a player.", MessageType.Info);
            EditorGUILayout.PropertyField(playerActionsProp, true);
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("The list of bullet patterns the enemy can play during the battle.", MessageType.Info);
            EditorGUILayout.PropertyField(bulletPatternsProp, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
