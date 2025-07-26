using System.Collections.Generic;

namespace Core.Combat.Actions
{
    public abstract class BattleAction
    {
        public enum SpellTargetType
        {
            Players,
            Enemies
        }
        
        protected Battle battle;
        protected List<BattleSequence> battleSequence;
        protected List<BattleSequence> enemySequence;
        public virtual SpellTargetType TargetTypeType => SpellTargetType.Enemies;
        
        public BattleAction(Battle battle)
        {
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
