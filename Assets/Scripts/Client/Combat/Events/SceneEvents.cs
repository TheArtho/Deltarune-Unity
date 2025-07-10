namespace Client.Combat.Events
{
    public interface IBattleSceneEvents
    {
    
    }

    public class PlayAnim : IBattleSceneEvents
    {
        public string animation;
    }

    public class BulletHellEndedEvent : IBattleSceneEvents
    {
        public int Player;
    }
}
