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
            battleSequence.Add(new TextSequence
            {
                Text = $"Bro thought I had time to code Rude Buster lol.",
                Delay = 1
            });
        }
    }
}