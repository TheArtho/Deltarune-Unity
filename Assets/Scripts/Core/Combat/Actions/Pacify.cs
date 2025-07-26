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
            if (battle.Enemies[target].status == Status.Tired)
            {
                battle.Enemies[target].IsSpared = true;
                        
                battleSequence.Add(new PlayerAnimationSequence
                {
                    RunInParallel = true,
                    Player = user,
                    Animation = "Act"
                });
                battleSequence.Add(new TextSequence
                {
                    Text = $"{battle.Players[user].name} pacified {battle.Enemies[target].name}.",
                    Delay = 0.5f
                });
                battleSequence.Add(new SpareEnemySequence
                {
                    Enemy = target,
                });
            }
            else
            {
                battleSequence.Add(new PlayerAnimationSequence
                {
                    RunInParallel = true,
                    Player = user,
                    Animation = "Act"
                });
                battleSequence.Add(new TextSequence
                {
                    Text = $"{battle.Players[user].name} pacified {battle.Enemies[target].name}.",
                    Delay = 0.5f
                });
                battleSequence.Add(new TextSequence
                {
                    Text = $"\nBut its name wasn't <gradient=pacify>BLUE</gradient>...",
                    ClearText = false
                });
            }
        }
    }
}