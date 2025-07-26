namespace Core.Combat.Actions
{
    public abstract class BattleActionQte : BattleAction
    {
        public BattleActionQte(Battle battle) : base(battle)
        {
        }

        public override void Execute(int user, int target)
        {
            
        }
    }
}