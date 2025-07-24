namespace Core.Combat
{
    public interface IBattleSequence
    {
   
        
    }

    public class BattleSequence : IBattleSequence
    {
        public bool RunInParallel = false;
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
        public bool clearText = true;
        public bool canSkip = false;
        public float delay = 1f;
    }

    public class PlayerAnimationSequence : BattleSequence
    {
        public int character;
        public string animation;
        public float time;
    }
}