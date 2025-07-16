namespace Client.Combat.Events
{
    public interface IBattleSceneEvents
    {
    
    }

    public class BattleSequenceEnded : IBattleSceneEvents
    {
        public int Player;
    }

    public class FightQteEnded : IBattleSceneEvents
    {
        public int Player;
    }
    
    public class BulletHellReadyEvent : IBattleSceneEvents
    {
        public int Player;
    }

    public class BulletHellEndedEvent : IBattleSceneEvents
    {
        public int Player;
    }
}
