using System.Collections.Generic;

namespace Core.Combat.Actions
{
    public abstract class BattleItem
    {
        public enum TargetType
        {
            Specific,
            All
        }
        
        protected Battle battle;
        protected List<BattleSequence> battleSequence;
        protected List<BattleSequence> enemySequence;
        
        public virtual TargetType Target => TargetType.Specific;
        public bool Selected;
        
        public BattleItem(Battle battle)
        {
            this.battle = battle;
            this.battleSequence = battle.BattleSequence;
            this.enemySequence = battle.EnemySequence;
        }

        public abstract string GetName();

        public abstract void Use(int user, int target);
    }
}