namespace Core.Combat
{
    public interface IBattleSequence
    {
   
        
    }

    public class BattleSequence : IBattleSequence
    {
        public bool RunInParallel;
    }

    public class DialogSequence : BattleSequence
    {
    
    }

    public class TargetSequence : BattleSequence
    {
    
    }
}