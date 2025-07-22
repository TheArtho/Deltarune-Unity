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
        public int character;
        public string text;
    }

    public class TextSequence : BattleSequence
    {
        public string text;
        public string sound = "text";
        public bool canSkip;
    }

    public class PlayerAnimationSequence : BattleSequence
    {
        public int character;
        public string animation;
        public float time;
    }
}