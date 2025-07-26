using Core.Combat.Actions;

namespace Core.Combat.Items
{
    [ItemClass(nameof(Darkburger))]
    public class Darkburger : BattleItem
    {
        private string name = "Darkburger";

        private int healAmount = 70;
    
        public Darkburger(Battle battle) : base(battle)
        {
        }

        public override string GetName()
        {
            return name;
        }

        public override void Use(int user, int target)
        {
            battle.Players[target].Heal(healAmount);
        
            battleSequence.Add(new TextSequence
            {
                Text = $"{battle.Players[user].name} used {name}!",
                Delay = -1
            });
            battleSequence.Add(new PlayerAnimationSequence
            {
                Character = user,
                Animation = "UseItem",
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
