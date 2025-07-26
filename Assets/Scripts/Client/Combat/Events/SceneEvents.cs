namespace Client.Combat.Events
{
    public interface IBattleSceneEvent
    {
    
    }

    public class BattleSequenceEnded : IBattleSceneEvent
    {
        public int Player;
    }

    public class FightQteEnded : IBattleSceneEvent
    {
        public int Player;
    }
    
    public class BulletHellReadyEvent : IBattleSceneEvent
    {
        public int Player;
    }

    public class BulletHellEndedEvent : IBattleSceneEvent
    {
        public int Player;
    }

    public class PlayerHurtEvent : IBattleSceneEvent
    {
        public int Player;
        public int Damage;
    }
    
    public class GrazeEvent : IBattleSceneEvent
    {
        public int Player;
    }

    public class EndBattleReadyEvent : IBattleSceneEvent
    {
        public int Player;
    }
}
