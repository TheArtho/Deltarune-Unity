namespace Core.Combat.Events
{
    public interface IBulletHellEvent { }

    public class TurnStartedEvent : IBulletHellEvent { }

    public class ActorDamagedEvent : IBulletHellEvent
    {
        public int ActorId;
        public int Amount;
    }

    public class BulletPhaseStartedEvent : IBulletHellEvent { }
}