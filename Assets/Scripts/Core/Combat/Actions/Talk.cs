namespace Core.Combat.Actions
{
    [ActionClass(nameof(Talk))]
    public class Talk : BattleAction
    {
        private string name = "Talk";
        
        public Talk(Battle battle) : base(battle)
        {
        }

        public override string GetName()
        {
            return name;
        }

        public override void Execute(int user, int target)
        {
            battleSequence.Add(new PlayerAnimationSequence
            {
                RunInParallel = true,
                Player = user,
                Animation = "Act"
            });
            battleSequence.Add(new TextSequence
            {
                Text = $"{battle.Players[user].name} is talking to {battle.Enemies[target].name}."
            });
        }
    }
}