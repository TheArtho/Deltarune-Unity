namespace Client.Combat.Events
{
    public interface IBattleInterfaceEvent
    {
    
    }
    
    public class PlayerCommandEvent : IBattleInterfaceEvent
    {
        public int Player;
        public ActionType ActionType;
        public int TargetId;
        public int Index; // The index of the action for act, magic and item (example: index of the action)
    }
    
    public class PlayerCancelCommandEvent : IBattleInterfaceEvent
    {
        public int Player;
    }

    public class AnsFightQuickTimeEvent : IBattleInterfaceEvent
    {
        public int Player;
        public int Accuracy;
    }
}
