using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "New Prefab List", menuName = "Scriptable Objects/Prefab List")]
    public class PrefabList : ScriptableObject
    {
        [FormerlySerializedAs("soulControllers")] [SerializeField] private List<GameObject> prefabs;

        public GameObject GetPrefab(string identifier)
        {
            return prefabs
                .FirstOrDefault(go => go.name == identifier);
        }
    }
}