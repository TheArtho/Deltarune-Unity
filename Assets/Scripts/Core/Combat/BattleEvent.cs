namespace Core.Combat
{
    public class BattleEvent
    {
        public BattleEventType Type;
        public object Payload;
    }

    public class BattleInterfaceEvent
    {
        public BattleInterfaceEventType Type;
        public object Payload;
    }

    public enum BattleEventType
    {
        TurnStarted,
        ActorDamaged,
        ActorHealed,
        ActorDown,
        ActorSpared,
        TpChanged,
        BulletPhaseStarted,
        BulletHitPlayer,
        BulletNearPlayer,
        BulletPhaseEnded,
        TurnEnded
    }

    public enum BattleInterfaceEventType
    {
        PlayAnimation,
        ShowDialog,
        PlaySound,
        SpawnEffect,
        ChooseAction,
        CancelAction,
        Fight,
        Act,
        Item
    }
}