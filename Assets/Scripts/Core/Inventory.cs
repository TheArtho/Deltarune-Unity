using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class Inventory
    {
        [SerializeField] private List<ItemDefinition> items;
        public List<ItemDefinition> Items => items;

        public string[] GetItemNames()
        {
            return items.Select(x => x.name).ToArray();
        }
    }
}
