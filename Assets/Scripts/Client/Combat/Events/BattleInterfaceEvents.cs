namespace Client.Combat.Events
{
    public interface IBattleInterfaceEvents
    {
    
    }
    
    public class PlayerCommandEvent : IBattleInterfaceEvents
    {
        public int Player;
        public ActionType ActionType;
        public int TargetId;
        public int Index; // The index of the action for act, magic and item (example: index of the action)
    }
    
    public class PlayerCancelCommandEvent : IBattleInterfaceEvents
    {
        public int Player;
    }

    public class AnsFightQuickTimeEvent : IBattleInterfaceEvents
    {
        public int Player;
        public int Accuracy;
    }
}
