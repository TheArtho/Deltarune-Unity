using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Combat.Actions;
using Scriptables;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "ActionDefinition", menuName = "Scriptable Objects/ActionDefinition")]
    public class ActionDefinition : ScriptableObject
    {
        private static readonly Dictionary<string, Type> actionTypes;
        
        [SerializeField, HideInInspector] public string actionClassName; // ex: "Check"
        
        static ActionDefinition()
        {
            actionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(BattleAction).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ActionClassAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr.ActionName, x => x.Type);
        }

        public BattleAction CreateInstance(Battle battle)
        {
            var type = actionTypes[actionClassName];
            if (type == null)
            {
                Debug.LogError($"Action class not found: {actionClassName}");
                return null;
            }

            return (BattleAction)Activator.CreateInstance(type, battle);
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ActionClassAttribute : Attribute
{
    public string ActionName { get; }

    public ActionClassAttribute(string actionName)
    {
        ActionName = actionName;
    }
}