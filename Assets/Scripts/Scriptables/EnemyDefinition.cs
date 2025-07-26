using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Enemy Definition", menuName = "Scriptable Objects/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        private static readonly Dictionary<string, Type> enemyTypes;
        
        [FormerlySerializedAs("actionClassName")] [SerializeField, HideInInspector] public string enemyClassName; // ex: "Enemy"
        [SerializeField] public BattleSprite battleSpritePrefab;
        
        [Serializable]
        public class ActionList
        {
            public string characterId;
            public List<ActionDefinition> actions;
        }
        
        [Header("Parameters")]
        [SerializeField] public string name;
        [SerializeField] public int hp;
        [SerializeField] public int attack;
        [SerializeField] public int defense;
        [SerializeField] public bool haveMercy;
        
        [SerializeField] public List<ActionList> playerActions;
        
        static EnemyDefinition()
        {
            enemyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Enemy).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<ActionClassAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr.ClassName, x => x.Type);
        }

        public Enemy CreateInstance()
        {
            var type = Type.GetType(enemyClassName);
            if (type == null)
            {
                Debug.LogError($"Action class not found: {enemyClassName}");
                return null;
            }

            // Cherche un constructeur Enemy(EnemyDefinition)
            var ctor = type.GetConstructor(new[] { typeof(EnemyDefinition) });
            if (ctor != null)
                return (Enemy)ctor.Invoke(new object[] { this });
            
            return (Enemy)Activator.CreateInstance(type);
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class EnemyClassAttribute : Attribute
{
    public string ActionName { get; }

    public EnemyClassAttribute(string actionName)
    {
        ActionName = actionName;
    }
}