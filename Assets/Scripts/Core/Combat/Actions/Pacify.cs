namespace Core.Combat.Actions
{
    [ActionClass(nameof(Pacify))]
    public class Pacify : BattleAction
    {
        private string name = "Pacify";
        
        public Pacify(Battle battle) : base(battle)
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