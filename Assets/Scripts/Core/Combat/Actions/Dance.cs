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
            battleSequence.Add(new PlayerAnimationSequence
            {
                RunInParallel = true,
                Player = user,
                Animation = "Dance",
                LockAnimation = true
            });
            battleSequence.Add(new TextSequence
            {
                Text = $"{battle.Players[user].name} started dancing\nfor no reason.",
            });
        }
    }
}