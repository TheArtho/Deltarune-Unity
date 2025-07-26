namespace Core.Combat.Actions
{
    [ActionClass(nameof(HealPrayer))]
    public class HealPrayer : BattleAction
    {
        private string name = "Heal Prayer";
        
        public HealPrayer(Battle battle) : base(battle)
        {
        }

        public override string GetName()
        {
            return name;
        }

        public override void Execute(int user, int target)
        {
            
        }
    }
}