using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Combat.Actions;
using Scriptables;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "ActionDefinition", menuName = "Scriptable Objects/Action Definition")]
    public class ActionDefinition : ScriptableObject
    {
        private static readonly Dictionary<string, Type> ActionTypes;
        
        [SerializeField, HideInInspector] public string actionClassName; // ex: "Check"
        
        static ActionDefinition()
        {
            ActionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(BattleAction).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ActionClassAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr.ClassName, x => x.Type);
        }

        public BattleAction CreateInstance(Battle battle)
        {
            var type = ActionTypes[actionClassName];
            if (type == null)
            {
                Debug.LogError($"Action class not found: {actionClassName}");
                return null;
            }

            Debug.Log("Creating instance of BattleAction with type : " + actionClassName);
            
            return (BattleAction)Activator.CreateInstance(type, battle);
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ActionClassAttribute : Attribute
{
    public string ClassName { get; }

    public ActionClassAttribute(string className)
    {
        ClassName = className;
    }
}