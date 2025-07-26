namespace Core.Combat.Actions
{
    [ActionClass(nameof(RudeBuster))]
    public class RudeBuster : BattleAction
    {
        private string name = "Rude Buster";
        
        public RudeBuster(Battle battle) : base(battle)
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