using Core.Combat;
using Core.Combat.Actions;

[ItemClass(nameof(TopCake))]
public class TopCake : BattleItem
{
    private string name = "Top Cake";
    
    private int healAmount = 160;
    
    public override TargetType Target => TargetType.All;
    
    public TopCake(Battle battle) : base(battle)
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
            Player = user,
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
