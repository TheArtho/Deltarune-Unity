namespace Core.Combat.Actions
{
    [ActionClass(nameof(Check))]
    public class Check : BattleAction
    {
        private string name = "Check";
        
        public Check(Battle battle) : base(battle)
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
                Character = user,
                Animation = "Act"
            });
            battleSequence.Add(new TextSequence
            {
                Text = $"{battle.Enemies[target].name.ToUpperInvariant()} - ATK {battle.Enemies[target].attack} DF {battle.Enemies[target].defense}",
                Delay = 0
            });
            battleSequence.Add(new TextSequence
            {
                ClearText = false,
                Text = $"\n{battle.Enemies[target].description}"
            });
        }
    }
}
