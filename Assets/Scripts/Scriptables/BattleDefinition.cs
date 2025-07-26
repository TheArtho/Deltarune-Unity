using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "BattleDefinition", menuName = "Scriptable Objects/BattleDefinition")]
    public class BattleDefinition : ScriptableObject
    {
        public string introText = "A monster appears.";
        public string music = "rude_buster";
        public List<CharacterDefinition> players;
        public List<EnemyDefinition> enemies;
    }
}
