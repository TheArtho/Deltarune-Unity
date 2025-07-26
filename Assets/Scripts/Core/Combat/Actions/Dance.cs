namespace Core.Combat.Actions
{
    [ActionClass(nameof(Dance))]
    public class Dance : BattleAction
    {
        private string name = "Dance";
        
        public Dance(Battle battle) : base(battle)
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