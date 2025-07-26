using System.Collections.Generic;
using UnityEngine;

namespace Core.Combat.Actions
{
    public abstract class BattleAction
    {
        protected Battle battle;
        protected List<BattleSequence> battleSequence;
        protected List<BattleSequence> enemySequence;
        public BattleAction(Battle battle)
        {
            Debug.Log("BattleAction created");
            this.battle = battle;
            this.battleSequence = battle.BattleSequence;
            this.enemySequence = battle.EnemySequence;
        }

        public abstract string GetName();

        public virtual int GetTpCost()
        {
            return 0;
        }

        public abstract void Execute(int user, int target);
    }
}
