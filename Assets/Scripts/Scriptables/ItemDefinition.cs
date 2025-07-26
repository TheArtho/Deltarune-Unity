using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Combat.Actions;
using Scriptables;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Scriptable Objects/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        private static readonly Dictionary<string, Type> ItemTypes;
        
        [SerializeField, HideInInspector] public string itemClassName; // ex: "Darkburger"
        
        static ItemDefinition()
        {
            ItemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(BattleItem).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ItemClassAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr.ClassName, x => x.Type);
        }

        public BattleItem CreateInstance(Battle battle)
        {
            var type = ItemTypes[itemClassName];
            if (type == null)
            {
                Debug.LogError($"Item class not found: {itemClassName}");
                return null;
            }

            return (BattleItem)Activator.CreateInstance(type, battle);
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ItemClassAttribute : Attribute
{
    public string ClassName { get; }

    public ItemClassAttribute(string className)
    {
        ClassName = className;
    }
}