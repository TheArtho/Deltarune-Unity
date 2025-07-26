using System;

namespace Core.Combat.Actions
{
    [ActionClass(nameof(HealPrayer))]
    public class HealPrayer : BattleAction
    {
        private string name = "Heal Prayer";
        public override SpellTargetType TargetTypeType => SpellTargetType.Players;
        
        public HealPrayer(Battle battle) : base(battle)
        {
        }

        public override string GetName()
        {
            return name;
        }

        public override void Execute(int user, int target)
        {
            int healAmount = Math.Max(1, battle.Players[user].magic * 5);
            battle.Players[target].Heal(healAmount);
        
            battleSequence.Add(new TextSequence
            {
                Text = $"{battle.Players[user].name} used {name}!",
                Delay = 0
            });
            battleSequence.Add(new PlayerAnimationSequence
            {
                Player = user,
                Animation = "Act",
                Time = 0.8f
            });
            battleSequence.Add(new HealPlayerSequence
            {
                Player = target,
                CurrentHp = battle.Players[target].hp,
                HealAmount = healAmount
            });
            battleSequence.Add(new ClearTextSequence());
        }
    }
}